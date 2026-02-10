using CommunityToolkit.Mvvm.Input;
using RequestFiend.Models.Messages;
using RequestFiend.Models.PropertyTypes;
using RequestFiend.Models.Services;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace RequestFiend.Models;

public partial class PreferencesModel : BoundModelBase {
    private readonly IPreferencesService preferencesService;
    private readonly IMessageService messageService;
    private readonly IPopupService popupService;

    public string PageTitle { get => field; set => SetProperty(ref field, value); }
    public string ShellItemTitle { get => field; set => SetProperty(ref field, value); }
    public ValidatableProperty<bool> ShowRecentCollections { get; set; }
    public ValidatableProperty<string?> MaximumRecentCollectionCount { get; set; }

    public PreferencesModel(IPreferencesService preferencesService, IMessageService messageService, IPopupService popupService) {
        this.preferencesService = preferencesService;
        this.messageService = messageService;
        this.popupService = popupService;

        ShowRecentCollections = new(() => preferencesService.GetShowRecentCollections());
        MaximumRecentCollectionCount = new(() => preferencesService.GetMaximumRecentCollectionCount().ToString(), Validator.Numeric);

        ConfigureState([ShowRecentCollections, MaximumRecentCollectionCount], []);
        UpdateTitles();
        PropertyChanged += (_, e) => {
            if (e.PropertyName == nameof(IsModified) || e.PropertyName == nameof(HasError)) {
                UpdateTitles();
            }
        };
    }

    [MemberNotNull(nameof(PageTitle), nameof(ShellItemTitle))]
    public void UpdateTitles() {
        var suffix = HasError ? " ▲" : IsModified ? " ●" : "";

        PageTitle = ShellItemTitle = $"Preferences{suffix}";
    }

    [RelayCommand]
    public void Update() {
        if (HasError) {
            return;
        }

        preferencesService.SetShowRecentCollections(ShowRecentCollections.Value);
        preferencesService.SetMaximumRecentCollectionCount(int.Parse(MaximumRecentCollectionCount.Value!));

        ShowRecentCollections.Reset();
        MaximumRecentCollectionCount.Reset();

        if (ShowRecentCollections.Value) {
            preferencesService.TrimRecentCollections();
        }
        else {
            preferencesService.ClearRecentCollections();
        }

        messageService.Send(new SuccessMessage("Preferences have been updated"));
    }

    [RelayCommand]
    public async Task Reset() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to reset your preferences?")) {
            preferencesService.Reset();
            ShowRecentCollections.Reset();
            MaximumRecentCollectionCount.Reset();

            messageService.Send(new SuccessMessage("Preferences have been reset"));
        }
    }

    [RelayCommand]
    public async Task ClearRecentCollections() {
        if (await popupService.ShowConfirmPopup("Are you sure you want to clear your recent collections?")) {
            preferencesService.ClearRecentCollections();

            messageService.Send(new SuccessMessage("Recent collections have been cleared"));
        }
    }
}
