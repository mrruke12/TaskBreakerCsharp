namespace TaskBreakerApi.Services {
    public class UserRepository {
        private AppDbContext _context { get; init; }

        public UserRepository(AppDbContext context) {        
            _context = context;
        }

        public async Task<User?> GetById(int id) {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmail(string Email) {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
        }

        public async Task<bool> Any(User user) {
            return await _context.Users.AnyAsync(u => u == user);
        }

        public async Task<bool> AnyEmail(string Email) {
            return await _context.Users.AnyAsync(u => u.Email == Email);
        }

        public async Task<User> Register(UserRegistration userReg) {
            User user = new User {
                Email = userReg.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userReg.PasswordHash)
            };

            await _context.Users.AddAsync(user); // предполагается, что проверка на наличие пользователя с такой почтой будет проводится до вызова этой функции, поэтому считается, что такого пользователя еще нет и его можно добавить
            await _context.SaveChangesAsync();

            return await _context.Users.FirstAsync(u => u.Email == user.Email);
        }
        
        public async Task<bool> CheckPassword(string Email, string Password) {
            var user = await _context.Users.FirstAsync(u => u.Email == Email); // предполагается, что проверка на наличие пользователя с такой почтой будет проводится до вызова этой функции
            return BCrypt.Net.BCrypt.Verify(Password, user.PasswordHash);
        }

        public async Task<bool> Delete(int id) {
            User user = await GetById(id);

            if (user != null) {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> Update(User user) {
            _context.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
