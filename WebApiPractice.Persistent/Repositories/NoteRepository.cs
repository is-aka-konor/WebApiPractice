using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WebApiPractice.Persistent.Context;
using WebApiPractice.Persistent.DataModels;

namespace WebApiPractice.Persistent.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private readonly AppDbContext _appDbContext;

        public NoteRepository(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        public async Task<Note> GetNoteByExternalId(Guid externalId)
        {
            return await this._appDbContext.Notes.FirstOrDefaultAsync(x => x.NoteExternalId.Equals(externalId)).ConfigureAwait(false);
        }

        public async Task<Note> SaveNote(Note note)
        {
            note.RowVersion = RowVersionGenerator.GetVersion();
            note.UpdateAt = DateTime.Now;
            await this._appDbContext.Notes.AddAsync(note).ConfigureAwait(false);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return note;
        }

        public async Task<Note> UpdateNote(Note note)
        {
            note.RowVersion = RowVersionGenerator.GetVersion();
            note.UpdateAt = DateTime.Now;
            this._appDbContext.Notes.Update(note);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return note;
        }
    }
}
