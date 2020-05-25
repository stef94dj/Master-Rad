using MasterRad.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Repositories
{
    public interface IExerciseRepository
    {
        ExerciseInstanceEntity Get(int id);
        ExerciseInstanceEntity GetAsTracking(int id);
        ExerciseInstanceEntity GetWithTemplate(int id);
        IEnumerable<ExerciseInstanceEntity> GetInstancesWithTemplates(Guid studentId);
        bool NameExists(string name, Guid studentId);
        bool Create(int templateId, Guid studentId, string name, string nameOnServer, Guid userId);
        bool Delete(int id, byte[] timeStamp);
        bool UpdateSql(ExerciseInstanceEntity entity, byte[] requestTimeStamp, string sql);
    }

    public class ExerciseRepository : IExerciseRepository
    {
        private readonly Context _context;

        public ExerciseRepository(Context context)
        {
            _context = context;
        }

        public ExerciseInstanceEntity Get(int id)
            => _context.Exercises
                       .AsNoTracking()
                       .Single(x => x.Id == id);

        public ExerciseInstanceEntity GetAsTracking(int id)
            => _context.Exercises
                       .Single(x => x.Id == id);

        public ExerciseInstanceEntity GetWithTemplate(int id)
            => _context.Exercises
                       .AsNoTracking()
                       .Include(x => x.Template)
                       .Single(x => x.Id == id);

        public IEnumerable<ExerciseInstanceEntity> GetInstancesWithTemplates(Guid studentId)
            => _context.Exercises
                       .Include(x => x.Template)
                       .Where(x => x.StudentId == studentId);

        public bool NameExists(string name, Guid studentId)
            => _context.Exercises
                        .Any(e => e.StudentId == studentId && e.Name.ToLower().Equals(name.ToLower()));

        public bool Create(int templateId, Guid studentId, string name, string nameOnServer, Guid userId)
        {
            var entity = new ExerciseInstanceEntity() //AutoMapper
            {
                TemplateId = templateId,
                StudentId = studentId,
                Name = name,
                NameOnServer = nameOnServer,
                DateCreated = DateTime.UtcNow,
                CreatedBy = userId,
            };

            _context.Exercises.Add(entity);
            return _context.SaveChanges() == 1;
        }

        public bool Delete(int id, byte[] timeStamp)
        {
            var entity = new ExerciseInstanceEntity()
            {
                Id = id,
                TimeStamp = timeStamp
            };

            _context.Exercises.Remove(entity);
            return _context.SaveChanges() == 1;
        }

        public bool UpdateSql(ExerciseInstanceEntity entity, byte[] requestTimeStamp, string sql)
        {
            entity.TimeStamp = requestTimeStamp;
            entity.SqlScript = sql;
            return _context.SaveChanges() == 1;
        }
    }
}
