using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementApp.Helpers.Context;
using ProjectManagementApp.Model.Entity;

namespace ProjectManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly ProjectDbContext _dbcontext;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(ProjectDbContext dbcontext, ILogger<ProjectController> logger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
        }

        #region Project CRUD
        [HttpGet]
        public async Task<JsonResult> GetAllProjects()
        {
            try
            {
                var vprojects = await _dbcontext.mstProjecttbl
                                           .Where(p => p.isActive)
                                           .OrderByDescending(p => p.CreatedDate)
                                           .ToListAsync();
                if (vprojects == null || vprojects.Count == 0)
                {
                    return new JsonResult(new { success = false, message = "No projects found." });
                }
                return new JsonResult(new { success = true, data = vprojects });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching projects.");
                return new JsonResult(new { success = false, message = "An error occurred while fetching projects." });
            }
        }

        [HttpPost]
        public async Task<JsonResult> SaveProject([FromBody] mstProjecttbl project)
        {
            try
            {
                if (project == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid project data." });
                }
                project.CreatedDate ??= DateTime.Now;

                _dbcontext.mstProjecttbl.Add(project);
                await _dbcontext.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Project saved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the project.");
                return new JsonResult(new { success = false, message = "An error occurred while saving the project." });
            }
        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetProjectById(int id)
        {
            try
            {
                var project = await _dbcontext.mstProjecttbl.FirstOrDefaultAsync(p => p.ProjectID == id && p.isActive);
                if (project == null)
                {
                    return new JsonResult(new { success = false, message = "Project not found." });
                }
                return new JsonResult(new { success = true, data = project });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the project by ID.");
                return new JsonResult(new { success = false, message = "An error occurred while fetching the project." });
            }
        }

        [HttpPut("{id}")]
        public async Task<JsonResult> UpdateProject(int id, [FromBody] mstProjecttbl project)
        {
            try
            {
                if (project == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid project data." });
                }

                var existingProject = await _dbcontext.mstProjecttbl.FindAsync(id);
                if (existingProject == null)
                {
                    return new JsonResult(new { success = false, message = "Project not found." });
                }

                existingProject.ProjectName = project.ProjectName;
                existingProject.Description = project.Description;
                existingProject.StartDate = project.StartDate;
                existingProject.EndDate = project.EndDate;
                existingProject.Priority = project.Priority;
                existingProject.Status = project.Status;
                existingProject.UpdatedDate = DateTime.Now;
                existingProject.UpdatedBy = project.UpdatedBy;

                _dbcontext.mstProjecttbl.Update(existingProject);
                await _dbcontext.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Project updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the project.");
                return new JsonResult(new { success = false, message = "An error occurred while updating the project." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<JsonResult> SoftDeleteProject(int id)
        {
            try
            {
                var existingProject = await _dbcontext.mstProjecttbl.FindAsync(id);
                if (existingProject == null)
                {
                    return new JsonResult(new { success = false, message = "Project not found." });
                }

                existingProject.isActive = false;
                existingProject.UpdatedDate = DateTime.Now;

                _dbcontext.mstProjecttbl.Update(existingProject);
                await _dbcontext.SaveChangesAsync();
                return new JsonResult(new { success = true, message = "Project deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the project.");
                return new JsonResult(new { success = false, message = "An error occurred while deleting the project." });
            }
        }
        #endregion
    }
}
