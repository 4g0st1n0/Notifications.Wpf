using System;

namespace Notifications.Wpf
{
    public interface INotificationManager
    {
        void Show(object content, string areaName = "", int uniqueId = 0, TimeSpan? expirationTime = null, Action onClick = null, Action onClose = null);
    }
}