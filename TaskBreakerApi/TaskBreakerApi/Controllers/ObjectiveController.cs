using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskBreakerApi.Repositories;
using TaskBreakerApi.Services;

namespace TaskBreakerApi.Controllers {
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ObjectiveController : Controller {
        private readonly IConfiguration _configuration;
        private readonly ObjectiveRepository _objectiveRepository;
        private readonly UserRepository _userRepository;

        public ObjectiveController(IConfiguration configuration, ObjectiveRepository objectiveRepository, UserRepository userRepository) {
            _configuration = configuration;
            _objectiveRepository = objectiveRepository;
            _userRepository = userRepository;
        }

        [HttpGet("objective/all")]
        public async Task<ActionResult<IEnumerable<Objective>>> GetAllObjectives() {
            return Ok(await _objectiveRepository.GetAllObjectives(int.Parse(User.FindFirst("id").Value)));
        }

        [HttpGet("objective/{id}")]
        public async Task<ActionResult<Objective>> GetObjective(int id) {
            var objective = await _objectiveRepository.GetObjective(id);

            if (objective == null) {
                return BadRequest(new {
                    message = "Задачи с таким айди не найдено"
                });
            }

            return Ok(objective);
        }

        [HttpGet("objective/{id}/details")]
        public async Task<ActionResult<ObjectiveWithElements>> GetDetails(int id) {
            var result = await _objectiveRepository.GetObjectiveWithElements(id);

            if (result == null) {
                return BadRequest(new {
                    message = "Задачи с таким айди не найдено"
                });
            }

            return Ok(result);
        }

        [HttpPost("objective/create")]
        public async Task<IActionResult> CreateObjective([FromBody] Objective objective) {
            if (string.IsNullOrEmpty(objective.Goal)) {
                return BadRequest(new {
                    field = nameof(objective.Goal),
                    message = "Цель обязательна"
                });
            }

            if (string.IsNullOrEmpty(objective.Description)) {
                return BadRequest(new {
                    field = nameof(objective.Description),
                    message = "Описание обязательно"
                });
            }

            if (objective.UserId == -1 || await _userRepository.GetById(objective.UserId) == null) {
                return BadRequest(new {
                    message = "Некорректный айди пользователя"
                });
            }

            try {
                var result = await _objectiveRepository.CreateObjective(objective);
                return CreatedAtAction(nameof(CreateObjective), new { id = result.Id }, result);
            } catch (Exception ex) {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpPost("objective/{id}/create")]
        public async Task<IActionResult> createElement(int id, [FromBody] OElement element) {
            if (string.IsNullOrEmpty(element.Description)) {
                return BadRequest(new {
                    field = nameof(element.Description),
                    message = "Описание обязательно"
                });
            }

            if (await _objectiveRepository.GetElement(element.ConnectedTo) == null) {
                return BadRequest(new {
                    message = "Некорректный айди родительского элемента"
                });
            }

            if (await _objectiveRepository.GetObjective(element.ObjectiveId) == null) {
                return BadRequest(new {
                    message = "Некорректный айди задачи элемента"
                });
            }

            try {
                var result = await _objectiveRepository.AddElement(element);
                return CreatedAtAction(nameof(createElement), new { id = result.Id }, result);
            } catch (Exception ex) {
                return BadRequest(new {
                    message = ex.Message
                });
            }
        }

        [HttpDelete("objective/{id}/delete")]
        public async Task<IActionResult> DeleteObjective(int id) {
            var result = await _objectiveRepository.DeleteObjective(id);

            if (result) return Ok();
            return BadRequest(new {
                message = "Задача с таким id не найдена"
            });
        }

        [HttpDelete("objective/{parentid}/{id}/delete")]
        public async Task<IActionResult> DeleteElement(int id) {
            var result = await _objectiveRepository.DeleteElement(id);

            if (result) return Ok();
            return BadRequest(new {
                message = "Элемента с таким id не найдено"
            });
        }

        [HttpPut("objective/{id}/update")]
        public async Task<IActionResult> UpdateObjective([FromBody] Objective objective) {
            var result = await _objectiveRepository.UpdateObjective(objective);

            if (result) return Ok();
            return BadRequest(new {
                message = "Задачи с таким id не найдено"
            });
        }

        [HttpPut("objective/{parentid}/{id}/update")]
        public async Task<IActionResult> UpdateElement([FromBody] OElement element) {
            var result = await _objectiveRepository.UpdateElement(element);

            if (result) return Ok();
            return BadRequest(new {
                message = "Элемента с таким id не найдено"
            });
        }
    }
}
