﻿namespace RRExpenseTracker.Server.Data.Interfaces
{
    public interface IAttachmentsRepository
    {
        Task AddAsync(Attachment attachment);
        Task<IEnumerable<Attachment>> GetUnusedAttachmentsAsync(int hours);
        Task DeleteAsync(string id, string uploadedByUserId);
        Task DeleteBatchAsync(IEnumerable<Attachment> attachments);
        Task<IEnumerable<Attachment>> GetByUrlsAsync(string[] urls);


    }
}