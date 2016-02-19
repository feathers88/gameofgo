using System;
using System.Threading.Tasks;
using WinRtUtility;

namespace GoG.Client.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Loads the object.
        /// </summary>
        /// <typeparam name="T">Must be a class.</typeparam>
        /// <param name="storageType">Location to load from.</param>
        /// <param name="handle">Storage key (e.g. filename) is handle + type name.</param>
        /// <returns></returns>
        Task<T> LoadAsync<T>(StorageType storageType, string handle = null)
            where T : class;

        /// <summary>
        /// Saves the object.
        /// </summary>
        /// <typeparam name="T">Must be a class.</typeparam>
        /// <param name="obj"></param>
        /// <param name="storageType">Location to save to.</param>
        /// <param name="handle">Storage key (e.g. filename) is handle + type name.</param>
        /// <returns></returns>
        Task SaveAsync<T>(T obj, StorageType storageType, string handle = null)
            where T : class;
    }

    public class FileStorageService : IStorageService
    {
        public async Task<T> LoadAsync<T>(StorageType storageType, string handle = null)
            where T : class
        {
            if (handle == null) handle = String.Empty;
            var objectStorageHelper = new ObjectStorageHelper<T>(storageType);
            var rval = await objectStorageHelper.LoadAsync(handle);

            return rval;
        }

        public async Task SaveAsync<T>(T obj, StorageType storageType, string handle = null)
            where T : class
        {
            if (handle == null) handle = String.Empty;
            var objectStorageHelper = new ObjectStorageHelper<T>(storageType);
            await objectStorageHelper.SaveAsync(obj, handle);
        }
    }
}
