using Microsoft.AspNetCore.Components;
using Radzen;
using StudentApp.Models;
using StudentApp.Services;

namespace StudentApp.Components.Dialogs
{
    public partial class EditDialog
    {
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
        private Student Student = new Student();
        [Inject] DialogService DialogService { get; set; } = default!;
        [Inject] StudentService StudentService { get; set; } = default!;
        [Inject] private NotificationService NotificationService { get; set; } = default!;
        private bool popup;
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
    }
}