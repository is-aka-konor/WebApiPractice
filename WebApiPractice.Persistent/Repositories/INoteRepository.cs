using System;
using System.Threading.Tasks;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public interface INoteRepository
    {
        Task<Note> GetNoteByExternalId(Guid externalId);
        Task<Note> SaveNote(Note note);
        Task<Note> UpdateNote(Note note);
    }
}