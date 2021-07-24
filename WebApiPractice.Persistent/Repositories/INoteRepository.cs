using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public interface INoteRepository
    {
        Task<Note> GetNoteByExternalId(Guid externalId, CancellationToken cancellationToken);
        Task<Note> SaveNote(Note note, CancellationToken cancellationToken);
        Task<Note> UpdateNote(Note note, CancellationToken cancellationToken);
        Task DeleteNote(Note note, CancellationToken cancellationToken);
    }
}