using Amazon.S3.Model;
using System.Collections.Generic;

namespace Accounts.Domain.DataAccess.S3.Base
{
    public interface IS3Helper<T> where T : class
    {
        IEnumerable<T> GetList(string keyName);
        T GetJsonFile(string keyName);
        IEnumerable<T> GetAll(string keyName);
        void Upload(string message, string keyName, string contentType = "application/json");
        void Delete(string keyName);
        ListObjectsV2Response ListObjectsInS3(string keyName);
        string GetAsString(string keyName);
    }
}
