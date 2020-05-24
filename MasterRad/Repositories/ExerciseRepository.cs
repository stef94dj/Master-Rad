using MasterRad.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterRad.Repositories
{
    public interface IExerciseRepository
    {
        bool NameExists(string name, Guid studentId);
        bool Create(int templateId, Guid studentId, string name, string nameOnServer, Guid userId);
    }

    public class ExerciseRepository : IExerciseRepository
    {
        private readonly Context _context;

        public ExerciseRepository(Context context)
        {
            _context = context;
        }

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
         
    }
}
