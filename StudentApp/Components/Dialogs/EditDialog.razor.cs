using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StudentApp.Models;
using StudentApp.Services;
using System;

namespace StudentApp.Components.Dialogs
{
    public partial class EditDialog
    {
        #region Parameters
        [Parameter]
        public string FirstName { get; set; } = default!;
        [Parameter]
        public string LastName { get; set; } = default!;
        [Parameter]
        public int NRC { get; set; }
        [Parameter]
        public string Address { get; set; } = default!;
        [Parameter]
        public string PhoneNumber { get; set; } = default!;
        [Parameter]
        public int ProgramId { get; set; } = default!;
        [Parameter]
        public int Id { get; set; } = default!;
        [Parameter]
        public Action UpdateUI { get; set; } = default!;
        [Parameter] 
        public IEnumerable<Programs>? Programs { get; set; }
        [Parameter]
        public string StudentId { get; set; } = default!;
        [Parameter]
        public int attachmentId { get; set; } = default!;
        [Parameter]
        public DateTime DateCreated { get; set; } = default!;  
        #endregion
        #region Services
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] StudentService StudentService { get; set; } = default!;
        [Inject] private NotificationService NotificationService { get; set; } = default!;
        [Inject] AttachmentService AttachmentService { get; set; } = default!;
        #endregion
        private Student Student = new Student();
        private bool popup;
        bool value;
        private IBrowserFile? selectedFile;
        private Attachment? attachment;
        [Parameter]
        public string AttachmentName { get; set; } = default!;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Student.FirstName = FirstName;
            Student.LastName = LastName;
            Student.Address = Address;
            Student.PhoneNumber = PhoneNumber;
            Student.Nrc = NRC;
            Student.ProgramId = ProgramId;
        }

        void OnChange(bool value, string name)
        {
            if(!value)
            {
                attachment = new Attachment();
            }
        }
        public async Task EditAsync()
        {
            string actionText = "edit";
            bool? result = await DialogService.Confirm($"Are you sure you want to {actionText} this record?", "Update", new ConfirmOptions() { CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true, Draggable = false, OkButtonText = "Yes", CancelButtonText = "No" });

            if (result != null)
            {
                if ((bool)result)
                {
                    try
                    {
                        Student.Id = Id;
                        Student.StudentId = StudentId;
                        var FirstName = TrimInput(Student.FirstName);
                        var LastName = TrimInput(Student.LastName);
                        Student.FirstName = FirstName;
                        Student.LastName = LastName;
                        Student.AttchmentId = attachmentId;
                        //Student.Attachment = attachment;
                       
                        if(attachment != null)
                        {
                            attachment.Id = attachmentId;
                            await AttachmentService.UpdateAttachmentAsync(attachmentId, attachment);
                            ShowNotification(new NotificationMessage
                            { Severity = NotificationSeverity.Success, Summary = "Success", Detail = "Attachment Updated Successfully!", Duration = 2000 });
                        }
                        Student.DateCreated = DateCreated;
                        Student.LastModified = DateTime.Now;
                        await StudentService.UpdateStudentAsync(Id, Student);
                        Student = new Student();
                        await InvokeAsync(UpdateUI);
                        StateHasChanged();
                        DialogService.Close();
                        ShowNotification(new NotificationMessage
                        { Severity = NotificationSeverity.Success, Summary = "Success", Detail = "Record Updated Successfully!", Duration = 2000 });
                        DialogService.Close();
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
            }

        }
        private void ClearFields()
        {
            Student = new Student();
            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Cleard",
                Detail = "",
                Duration = 2000
            });
        }
        protected void ShowNotification(NotificationMessage message)
        {
            NotificationService?.Notify(message);
        }
        void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
        {
            ShowNotification(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = "Please fill in all required fields before submitting",
                Duration = 4000
            });
        }
        private static string TrimInput(string value)
        {
            var inputValue = value?.ToString().Trim() ?? string.Empty;
            return inputValue;
        }
        private async void OnFileInput(InputFileChangeEventArgs args)
        {
            long maxFileSize = 5 * 1024 * 1024;
            selectedFile = args.File;
            if (selectedFile.Size > maxFileSize)
            {
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Warning",
                    Detail = "File size is greater that max allowed\n Please select a different file",
                    Duration = 4000
                });
                attachment = null;
            }
            else
            {
                attachment = new Attachment
                {
                    FileName = selectedFile.Name,
                    FileType = selectedFile.ContentType,
                    DateCreate = DateTime.Now,
                };
                AttachmentName = selectedFile.Name;
                using (var stream = selectedFile.OpenReadStream(maxFileSize))
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await stream.CopyToAsync(memoryStream);
                        attachment.FileContent = memoryStream.ToArray();
                    }
                }
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "File successfully selected",
                    Duration = 4000
                });
            }


        }
    }
}