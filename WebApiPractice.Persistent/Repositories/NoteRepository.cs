using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
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

        public async Task<Note> GetNoteByExternalId(Guid externalId, CancellationToken cancellationToken)
        {
            return await this._appDbContext.Notes.FirstOrDefaultAsync(x => x.NoteExternalId.Equals(externalId)).ConfigureAwait(false);
        }

        public async Task<Note> SaveNote(Note note, CancellationToken cancellationToken)
        {
            note.RowVersion = RowVersionGenerator.GetVersion();
            note.UpdateAt = DateTime.Now;
            await this._appDbContext.Notes.AddAsync(note).ConfigureAwait(false);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return note;
        }

        public async Task<Note> UpdateNote(Note note, CancellationToken cancellationToken)
        {
            note.RowVersion = RowVersionGenerator.GetVersion();
            note.UpdateAt = DateTime.Now;
            this._appDbContext.Notes.Update(note);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
            return note;
        }

        public async Task DeleteNote(Note note, CancellationToken cancellationToken)
        {
            this._appDbContext.Remove(note);
            await this._appDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
