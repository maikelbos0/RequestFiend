using System;

namespace RequestFiend.Models.Messages;

[Obsolete("Use IPopupService.ShowErrorPopup instead.")]
public record ErrorMessage(string Text);
