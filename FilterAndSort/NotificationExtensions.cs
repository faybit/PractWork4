public static class NotificationExtensions
{
    public static IEnumerable<Notification> FilterAndSort(
        this IEnumerable<Notification> notifications,
        NotificationFilterOptions options)
    {
        if (notifications == null) throw new ArgumentNullException(nameof(notifications));
        if (options == null) throw new ArgumentNullException(nameof(options));

        // Применяем фильтры
        var filteredNotifications = notifications.AsEnumerable();

        if (options.IsRead.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.IsRead == options.IsRead.Value);
        }

        if (options.Types != null && options.Types.Any())
        {
            filteredNotifications = filteredNotifications.Where(n => options.Types.Contains(n.Type));
        }

        if (!string.IsNullOrWhiteSpace(options.SearchText))
        {
            var searchText = options.SearchText.Trim().ToLower();
            filteredNotifications = filteredNotifications.Where(n =>
                n.Title.ToLower().Contains(searchText) ||
                (n.Content?.ToLower().Contains(searchText) ?? false));
        }

        if (options.MinPriority.HasValue)
        {
            filteredNotifications = filteredNotifications.Where(n => n.Priority >= options.MinPriority.Value);
        }

        // Применяем сортировку
        IOrderedEnumerable<Notification> sortedNotifications = null;

        if (options.SortBy.HasValue)
        {
            switch (options.SortBy.Value)
            {
                case SortNotificationBy.Date:
                    sortedNotifications = options.Descending
                        ? filteredNotifications.OrderByDescending(n => n.CreatedAt)
                        : filteredNotifications.OrderBy(n => n.CreatedAt);
                    break;

                case SortNotificationBy.Priority:
                    sortedNotifications = options.Descending
                        ? filteredNotifications.OrderByDescending(n => n.Priority)
                        : filteredNotifications.OrderBy(n => n.Priority);
                    break;

                case SortNotificationBy.Title:
                    sortedNotifications = options.Descending
                        ? filteredNotifications.OrderByDescending(n => n.Title)
                        : filteredNotifications.OrderBy(n => n.Title);
                    break;

                default:
                    throw new ArgumentException("Invalid SortBy value.");
            }
        }

        // Если сортировка не была применена, возвращаем исходный порядок
        return sortedNotifications ?? filteredNotifications;
    }
}