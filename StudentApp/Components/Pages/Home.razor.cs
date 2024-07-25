using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StudentApp.Components.Dialogs;
using StudentApp.JSServices;
using StudentApp.Models;
using StudentApp.Services;
using System;
using System.IO;
namespace StudentApp.Components.Pages
{
    public partial class Home
    {

        #region Services
        [Inject] StudentService StudentService { get; set; } = default!;
        [Inject] ProgramsService ProgramsService { get; set; } = default!;
        [Inject] private NotificationService NotificationService { get; set; } = default!;
        [Inject] StorageHelper storageHelper { get; set; } = default!;
        [Inject] IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] AttachmentService AttachmentService { get; set; } = default!;  
        #endregion
        #region List
        private IEnumerable<Programs>? programs = new List<Programs>();
        private IEnumerable<Student>? students = new List<Student>();
        IList<Student>? selectedStudents;
        private IEnumerable<Attachment>? attachments = new List<Attachment>();
        #endregion
        #region Objects
        private Student Student = new Student();
        private readonly Random random = new Random();
        private string StudentNumber = string.Empty;
        private Attachment? attachment;
        #endregion
        RadzenDataGrid<Student>? StudentDataGrid;
        private bool IsLoading;
        readonly DataGridEditMode editMode = DataGridEditMode.Single;
        private string Theme = "Default";
        private string ThemeValue = string.Empty;
        private readonly bool popup;
        private readonly double value;
        private string AttachmentInfo = string.Empty;
        private readonly bool visible;
        private string? nrcValue;
        readonly RadzenDataGrid<Student>? grid;
        readonly bool allowRowSelectOnRowClick = false;
        readonly RadzenUpload? upload;
        readonly RadzenUpload? uploadDD;
        byte[]? documentContent;
        string? documentMimeType;

        readonly bool cancelUpload = false;
        private readonly List<string> Themes = new List<string>
        {
            "Default", "Standard", "Dark", "Material", "Humanistic"
        };
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Theme = ThemeValue;
                await base.OnInitializedAsync();
                students = await StudentService.GetStudentsAsync();
                programs = await ProgramsService.GetProgramsAsync();
                students = students?.OrderByDescending(st => st.Id).ToList();
                attachments = await AttachmentService.GetAttachmentsAsync();
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                throw new Exception(ex.Message);
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            ThemeValue = await GetThemeValue();
            Theme = ThemeValue;
            StateHasChanged();
        }
        private async void ChangeTheme(object value)
        {
            var Value = $"{value}.css";
            await SetThemeValue(Value);
            _ = await GetThemeValue();
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
        private static string TrimInput(string value)
        {
            var inputValue = value?.ToString().Trim() ?? string.Empty;
            return inputValue;
        }
        private async void AddStudent(Student student)
        {
            try
            {
                if(attachment != null && attachment?.FileContent != null)
                {
                    IsLoading = true;
                    StudentNumber = $"{random.Next(1000) + 1:D4}";
                    student.StudentId = StudentNumber;
                    var FirstName = TrimInput(student.FirstName);
                    var LastName = TrimInput(student.LastName);
                    student.FirstName = FirstName;
                    student.LastName = LastName;
                    await AttachmentService.AddAttachmentAsync(attachment);
                    attachment = new Attachment();
                    ShowNotification(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Saving attachment",
                        Detail = "Please wait",
                        Duration = 4000
                    });
                    var latestAttachment = await AttachmentService.GetAttachmentsAsync();
                    var latestAttachmentId = latestAttachment?.Last().Id;
                    student.AttchmentId = latestAttachmentId;
                    await StudentService.AddStudentAsync(student);
                    ShowNotification(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Student addedd successfully",
                        Duration = 4000
                    });
                    IsLoading = false;
                    students = await StudentService.GetStudentsAsync();
                    students = students?.OrderByDescending(st => st.Id).ToList();
                    Student = new Student();
                    attachments = await AttachmentService.GetAttachmentsAsync();
                    StateHasChanged();
                }
                else
                {
                    ShowNotification(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Please add attachment.\n Please try again.",
                        Duration = 4000
                    });
                }
               
            }
            catch (Exception ex)
            {
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = $"{ex.Message}",
                    Duration = 4000
                });
                throw new Exception(ex.Message);
            }

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
        protected void ShowNotification(NotificationMessage message)
        {
            NotificationService?.Notify(message);
        }
        public async Task SetThemeValue(string ThemeValue)
        {
            await storageHelper.SetLocalStorage("Theme", ThemeValue);
        }
        public async Task<string> GetThemeValue()
        {
            ThemeValue = await storageHelper.GetLocalStorage("Theme");
            return ThemeValue;
        }
        private async Task OpenEditDialog(Student student, int Id)
        {
            await DialogService.OpenAsync<EditDialog>($"Editing - {student.FirstName} {student.LastName} - {student.StudentId}",
                         new Dictionary<string, object?>()
                         {
                              {"FirstName", student.FirstName},
                              {"LastName", student.LastName},
                              {"NRC", student.Nrc},
                              {"Address", student.Address},
                              {"PhoneNumber", student.PhoneNumber},
                              {"ProgramId", student.ProgramId},
                              {"Programs", programs},
                              {"StudentId", student.StudentId},
                              {"Id", Id},
                              {"UpdateUI", (Action)UpdateUI},
                         },
                         new DialogOptions() { Width = "1400px", Height = "max-content", Resizable = true, Draggable = true });
        }
        private async void DeleteStudent(int Id)
        {
            try
            {
                if (selectedStudents?.Count > 1)
                {
                    string actionText = "delete";
                    bool? result = await DialogService.Confirm($"Are you sure you want to {actionText} these record?", "Delete", new ConfirmOptions() { CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true, Draggable = false, OkButtonText = "Yes", CancelButtonText = "No" });

                    if (result != null)
                    {
                        if ((bool)result)
                        {

                            IsLoading = true;
                            foreach (var student in selectedStudents)
                            {
                                await StudentService.DeleteStudentAsync(student.Id);
                            }
                            ShowNotification(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success,
                                Summary = "Success",
                                Detail = "Records Removed Successfully",
                                Duration = 4000
                            });
                            IsLoading = false;
                            students = await StudentService.GetStudentsAsync();
                            students = students?.OrderByDescending(st => st.Id).ToList();
                            Student = new Student();
                            StateHasChanged();
                            DialogService.Close();
                        }
                    }
                }
                else
                {
                    string actionText = "delete";
                    bool? result = await DialogService.Confirm($"Are you sure you want to {actionText} this record?", "Delete", new ConfirmOptions() { CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true, Draggable = false, OkButtonText = "Yes", CancelButtonText = "No" });

                    if (result != null)
                    {
                        if ((bool)result)
                        {

                            IsLoading = true;
                            await StudentService.DeleteStudentAsync(Id);
                            ShowNotification(new NotificationMessage
                            {
                                Severity = NotificationSeverity.Success,
                                Summary = "Success",
                                Detail = "Record Removed Successfully",
                                Duration = 4000
                            });
                            IsLoading = false;
                            students = await StudentService.GetStudentsAsync();
                            students = students?.OrderByDescending(st => st.Id).ToList();
                            Student = new Student();
                            StateHasChanged();
                            DialogService.Close();
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private async void UpdateUI()
        {
            students = await StudentService.GetStudentsAsync();
            students = students?.OrderByDescending(st => st.Id).ToList();
            programs = await ProgramsService.GetProgramsAsync();
            StateHasChanged();
        }
        private IBrowserFile? selectedFile;
        private string? fileContent;
        private async void OnFileInput(InputFileChangeEventArgs args)
        {
            long maxFileSize = 5 * 1024 * 1024;
            selectedFile = args.File;
            if(selectedFile.Size > maxFileSize)
            {
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Warning,
                    Summary = "Warning",
                    Detail = "File size is greater that max allowed\n Please select a different file",
                    Duration = 4000
                });
                attachment = new Attachment();
            }
            else
            {
                attachment = new Attachment
                {
                    FileName = selectedFile.Name,
                    FileType = selectedFile.ContentType,
                };
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
        private async Task ViewSingleAttachedDocument(int? attchmentId)
        {
            try
            {
                if (attachments != null)
                {
                    IEnumerable<Attachment> _attachments = attachments.Where(at => at.Id == attchmentId);
                    foreach (var attachment in _attachments)
                    {
                        if (attachment.FileContent != null && attachment.FileContent.Length > 0)
                        {
                            documentContent = attachment.FileContent;
                            documentMimeType = attachment.FileType;

                            // Open the dialog

                        };
                        if (attachment.FileContent != null)
                            using (MemoryStream stream = new MemoryStream(attachment.FileContent))
                            {

                                stream.Position = 0;

                                await DialogService.OpenAsync<DocumentViewer>("Attachment",
                            new Dictionary<string, object?>()
                            {
                              {"DocumentContent", documentContent},
                              {"DocumentMimeType", documentMimeType},
                            },
                            new DialogOptions() { Width = "1400px", Height = "840px", Resizable = true, Draggable = true });

                            }
                    }
                }
                else 
                {
                    ShowNotification(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning,
                        Summary = "Attachment not available for this record",
                        Detail = "",
                        Duration = 4000
                    });
                }
            }
            catch (Exception ex)
            {
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 4000
                });
            }
        }
        private async Task DownloadSingleAttachedDocument(int? attachmentId)
        {
            try
            {
                if(attachments != null)
                {
                    IEnumerable<Attachment> _attachments = attachments.Where(at => at.Id == attachmentId);
                    foreach (var attachment in _attachments)
                    {
                        var mimeType = "application/octet-stream";
                        using (MemoryStream stream = new MemoryStream(attachment.FileContent))
                        {
                            stream.Position = 0;
                            await JSRuntime.InvokeVoidAsync("downloadFile", attachment.FileName, mimeType, attachment.FileContent);
                        }
                    }
                }
                else
                {
                    ShowNotification(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Warning,
                        Summary = "Unable to download attachment",
                        Detail = "",
                        Duration = 4000
                    });
                }
            }
            catch (Exception)
            {
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "An error occured",
                    Detail = "Download failed",
                    Duration = 4000
                });
            }
        }
        void OnChange(UploadChangeEventArgs args, string name)
        {
            foreach (var file in args.Files)
            {
                //console.Log($"File: {file.Name} / {file.Size} bytes");
            }

            //console.Log($"{name} changed");
        }
        //async Task ShowBusyDialog(bool withMessageAsString)
        //{
        //    InvokeAsync(async () =>
        //    {
        //        // Simulate background task
        //        await Task.Delay(2000);

        //        // Close the dialog
        //        DialogService.Close();
        //    });

        //    if (withMessageAsString)
        //    {
        //        await BusyDialog("Loading ...");
        //    }
        //    //else
        //    //{
        //    //    await BusyDialog();
        //    //}
        //}

        //// Busy dialog from string
        //async Task BusyDialog(string message)
        //{
        //    await DialogService.OpenAsync("", ds =>
        //    {
        //        RenderFragment content = b =>
        //        {
        //            b.OpenElement(0, "RadzenRow");

        //            b.OpenElement(1, "RadzenColumn");
        //            b.AddAttribute(2, "Size", "12");

        //            b.AddContent(3, message);

        //            b.CloseElement();
        //            b.CloseElement();
        //        };
        //        return content;
        //    }, new DialogOptions() { ShowTitle = false, Style = "min-height:auto;min-width:auto;width:auto", CloseDialogOnEsc = false });
        //}
    }
}