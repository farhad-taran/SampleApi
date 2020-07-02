using System.Collections.Generic;
using Books.Api.Contracts.Common;

namespace Books.Api.Domain
{
    public class ErrorTypes
    {
        private static readonly Dictionary<int, string> StorageErrorCodeMap = new Dictionary<int, string>
        {
            {1042,StorageUnAvailableErrorCode},
            {1062,StorageItemConflictErrorCode}
        };

        private static string MapStorageErrorCode(int errorCode)
        {
            var isMapped = StorageErrorCodeMap.TryGetValue(errorCode, out var mappedCode);
            return isMapped ? mappedCode : UnhandledStorageErrorCode;
        }

        //Api errors
        public const string InternalServerErrorCode = "errors.internal.server.error";

        public static Error InternalServerError
            => new Error(InternalServerErrorCode, "An unhandled application error has occured");

        //Storage errors
        public const string UnhandledStorageErrorCode = "errors.storage.unknown";
        public const string StorageItemNotFound = "errors.storage.item.notfound";
        public const string StorageUnAvailableErrorCode = "errors.storage.unavailable";
        public const string StorageItemConflictErrorCode = "errors.storage.item.conflict";
        public const string ValidationErrorCode = "errors.validation.error";

        public static Error FailedLocatingItem(string id)
            => new Error(StorageItemNotFound, $"Failed to locate an item with id: {id}");
        public static Error FailedLoadingItem(string id, int storageErrorCode)
            => new Error(MapStorageErrorCode(storageErrorCode), $"Failed to load item with id: {id}");
        public static Error FailedSavingItem(string storageMessage, int storageErrorCode)
            => new Error(MapStorageErrorCode(storageErrorCode), $"Failed to save item, {storageMessage}");
        public static Error FailedUpdatingItem(string id, string storageMessage, int storageErrorCode = 0)
            => new Error(MapStorageErrorCode(storageErrorCode), $"Failed to update item with id: {id}, {storageMessage}");
        public static Error FailedDeletingItem(string id, string storageMessage, int storageErrorCode = 0)
            => new Error(MapStorageErrorCode(storageErrorCode), $"Failed to delete item with id: {id}, {storageMessage}");
    }
}
