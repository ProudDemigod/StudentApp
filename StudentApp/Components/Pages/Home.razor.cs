using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StudentApp.Components.Dialogs;
using StudentApp.JSServices;
using StudentApp.Models;
using StudentApp.Services;
namespace StudentApp.Components.Pages
{
    public partial class Home
    {
        private Student Student = new Student();
        private IEnumerable<Programs>? programs = new List<Programs>();
        [Inject] StudentService StudentService { get; set; } = default!;
        [Inject] ProgramsService ProgramsService { get; set; } = default!;
        [Inject] private NotificationService NotificationService { get; set; } = default!;
        [Inject] StorageHelper storageHelper { get; set; } = default!;
        [Inject] DialogService DialogService { get; set; } = default!;
        private IEnumerable<Student>? students = new List<Student>();
        private readonly Random random = new Random();
        private string StudentNumber = string.Empty;
        RadzenDataGrid<Student>? StudentDataGrid;
        private bool IsLoading;
        readonly DataGridEditMode editMode = DataGridEditMode.Single;
        private string Theme = "Default";
        private string ThemeValue = string.Empty;
        private readonly bool popup;
        private readonly bool allowRowSelectOnRowClick = true;
        private readonly IList<Student>? SelectedStudents;
        private readonly double value;
        private string AttachmentInfo = string.Empty;
        private readonly bool visible;
        private string? nrcValue;

        readonly bool cancelUpload = false;
        private readonly List<string> Themes = new List<string>
        {
            "Default", "Standard", "Dark", "Material", "Humanistic"
        };
        protected override async Task OnInitializedAsync()
        {
            Theme = ThemeValue;
            await base.OnInitializedAsync();
            students = await StudentService.GetStudentsAsync();
            programs = await ProgramsService.GetProgramsAsync();
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
                IsLoading = true;
                StudentNumber = $"{random.Next(1000) + 1:D4}";
                student.StudentId = StudentNumber;
                var FirstName = TrimInput(student.FirstName);
                var LastName = TrimInput(student.LastName);
                student.FirstName = FirstName;
                student.LastName = LastName;
                await StudentService.AddStudentAsync(student);
                ShowNotification(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Student Created Successfully",
                    Duration = 4000
                });
                IsLoading = false;
                students = await StudentService.GetStudentsAsync();
                Student = new Student();
                StateHasChanged();
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
                string actionText = "delete";
                bool? result = await DialogService.Confirm($"Are you sure you want to {actionText} this record?", "Update", new ConfirmOptions() { CloseDialogOnOverlayClick = true, CloseDialogOnEsc = true, Draggable = false, OkButtonText = "Yes", CancelButtonText = "No" });

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
                            Detail = "Student Deleted Successfully",
                            Duration = 4000
                        });
                        IsLoading = false;
                        students = await StudentService.GetStudentsAsync();
                        Student = new Student();
                        StateHasChanged();
                        DialogService.Close();
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
            programs = await ProgramsService.GetProgramsAsync();
            StateHasChanged();
        }
    }
}