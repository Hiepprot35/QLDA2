using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using ProjectManager.Admin;
using ProjectManager.Admin.Shared;
using ProjectManager.Admin.Shared.Template;
using ProjectManager.Shared;
using ProjectManager.Shared.Common.Enum;
using ProjectManager.Shared.Model.Request;
using ProjectManager.Entity;
using BlazorInputFile;
using System;
using Microsoft.AspNetCore.Components;
using ProjectManager.Admin.Data;
using ProjectManager.Shared.Constants;
using ProjectManager.Shared.Model.ViewModel;
using Radzen;
using Radzen.Blazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ProjectManager.Admin.Pages.ProjectList
{
    public class ProjectListModalEditBase : CommonComponentBase
    {
        [Parameter]
        public RadzenDataGrid<ProjectListViewModel> grid { get; set; }

        [Parameter]
        public ProjectListViewModel projectlistViewModel { get; set; }

        [Parameter]
        public IEnumerable<Entity.Student> listStudent { get; set; }

        [Parameter]
        public IEnumerable<Entity.Teacher> listTeacher { get; set; }

        [Parameter]
        public IEnumerable<Entity.Specialized> listSpecialized { get; set; }

        public Entity.ProjectList editModel { get; set; } = new Entity.ProjectList();
        public bool isLoading;
        public bool isShow;
        protected override async Task OnInitializedAsync()
        {
            var teacher = await _teacherService.GetAllTeacherAsync(token);
            listTeacher = teacher.Data;
            var student = await _studentService.GetAllStudentAsync(token);
            listStudent = student.Data;
            isLoading = true;
            if (projectlistViewModel.Id > 0)
            {
                editModel.Id = projectlistViewModel.Id;
                editModel.Name = projectlistViewModel.Name;
                editModel.StudentId = projectlistViewModel.StudentId;
                editModel.TeacherId = projectlistViewModel.TeacherId;
                editModel.LinkDownload = projectlistViewModel.LinkDownload;
                editModel.Point = projectlistViewModel.Point;
                editModel.CreatedBy = projectlistViewModel.CreatedBy;
                editModel.CreatedDate = projectlistViewModel.CreatedDate;
                editModel.ModifiedBy = projectlistViewModel.ModifiedBy;
                editModel.ModifiedDate = projectlistViewModel.ModifiedDate;
                editModel.ID_ProjectList = projectlistViewModel.ID_ProjectList;
                isShow = true;
            }
            else
            {
                isShow = false;
            }

            await Delay();
            isLoading = false;
        }

        public void Cancel()
        {
            _dialogService.Close(true);
        }

        public void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
        }

        public async Task OnSubmit()
        {
            var regex = new Regex("^[a-zA-Z0-9\\p{L}\\s]*$");
            var isValid_name = regex.IsMatch(editModel.Name);
            var isValid_id = regex.IsMatch(editModel.ID_ProjectList);
            var message = new NotificationMessage();
            message.Duration = 4000;
            editModel.CreatedBy = userName;
            if (isValid_id && isValid_name)
            {
                if (editModel.Id > 0)
                {
                    editModel.ModifiedBy = userName;
                }

                try
                {
                    var result = await _projectListService.SaveAsync(editModel, token);
                    await grid.Reload();
                    if (result.ResponseCode == 200 && result.Data == true)
                    {
                        Cancel();
                        message.Severity = NotificationSeverity.Success;
                        message.Summary = Constants.Message.Successfully;
                        await grid.Reload();
                    }
                    else
                    {
                        message.Severity = NotificationSeverity.Error;
                        message.Summary = Constants.Message.Fail;
                    }

                    message.Detail = result.ResponseMessage;
                }
                catch (Exception)
                {
                    Cancel();
                    message.Severity = NotificationSeverity.Error;
                    message.Summary = Constants.Message.Fail;
                    message.Detail = Constants.Message.Idexist;
                    await grid.Reload();
                }
            }
            else
            {
                Cancel();
                message.Severity = NotificationSeverity.Error;
                message.Summary = Constants.Message.Fail;
                message.Detail = Constants.Message.Validation;
                await grid.Reload();
            }

            _notificationService.Notify(message);
        }
    }
    
}