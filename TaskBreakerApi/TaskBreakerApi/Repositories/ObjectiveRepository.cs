using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TaskBreakerApi.Services;

namespace TaskBreakerApi.Repositories {
    public class ObjectiveRepository {
        private readonly AppDbContext _context;

        public ObjectiveRepository(AppDbContext context) {
            _context = context;
        }

        public async Task<OElement> AddElement(OElement element) {
            await _context.OElements.AddAsync(element);
            await _context.SaveChangesAsync();
            return await _context.OElements.FirstAsync(e => e.Description == element.Description && e.Type == element.Type);
        }

        public async Task<Objective> CreateObjective(Objective obj) {
            await _context.Objectives.AddAsync(obj);
            await _context.SaveChangesAsync();
            return await _context.Objectives.FirstAsync(o => o.Goal == obj.Goal && o.Description == obj.Description);
        }

        public async Task<bool> DeleteObjective(int id) {
            Objective objective = await GetObjective(id);

            if (objective != null) {
                var elements = GetAllElements(objective.Id);
                if (elements != null) _context.RemoveRange(elements);

                _context.Remove(objective);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteElement(int id) {
            OElement element = await GetElement(id);

            if (element != null) {
                _context.Remove(element);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<OElement>> GetAllElements(int objectiveId) {
            return await _context.OElements.Where(e => e.ObjectiveId == objectiveId).ToListAsync();
        }

        public async Task<IEnumerable<Objective>> GetAllObjectives(int userId) {
            return await _context.Objectives.Where(o => o.UserId == userId).ToListAsync();
        }

        public async Task<OElement> GetElement(int id) {
            return await _context.OElements.FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Objective> GetObjective(int id) {
            return await _context.Objectives.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<ObjectiveWithElements> GetObjectiveWithElements(int id) {
            return new ObjectiveWithElements {
                objective = await _context.Objectives.FirstOrDefaultAsync(o => o.Id == id),
                elements = await _context.OElements.Where(e => e.ObjectiveId == id).ToListAsync()
            }; 
        }

        public async Task<bool> UpdateObjective(Objective obj) {
            _context.Objectives.Update(obj);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateElement(OElement obj) {
            _context.OElements.Update(obj);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
