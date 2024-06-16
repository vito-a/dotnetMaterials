public class TasksController : ApiController
{
    private TaskContext db = new TaskContext();

    // GET: api/Tasks
    public IQueryable<Task> GetTasks()
    {
        return db.Tasks;
    }

    // GET: api/Tasks/5
    [ResponseType(typeof(Task))]
    public async Task<IHttpActionResult> GetTask(int id)
    {
        Task task = await db.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        return Ok(task);
    }

    // PUT: api/Tasks/5
    [ResponseType(typeof(void))]
    public async Task<IHttpActionResult> PutTask(int id, Task task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (id != task.Id)
        {
            return BadRequest();
        }
        db.Entry(task).State = EntityState.Modified;
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TaskExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return StatusCode(HttpStatusCode.NoContent);
    }

    // POST: api/Tasks
    [ResponseType(typeof(Task))]
    public async Task<IHttpActionResult> PostTask(Task task)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return CreatedAtRoute("DefaultApi", new { id = task.Id }, task);
    }

    // DELETE: api/Tasks/5
    [ResponseType(typeof(Task))]
    public async Task<IHttpActionResult> DeleteTask(int id)
    {
        Task task = await db.Tasks.FindAsync(id);
        if (task == null)
        {
            return NotFound();
        }
        db.Tasks.Remove(task);
        await db.SaveChangesAsync();
        return Ok(task);
    }

    // Custom API to get tasks as XML
    [Route("api/Tasks/XmlReport")]
    public HttpResponseMessage GetTasksXmlReport()
    {
        var tasks = db.Tasks.ToList();
        var xmlDoc = new XDocument(
            new XElement("Tasks",
                tasks.Select(t => new XElement("Task",
                    new XElement("Id", t.Id),
                    new XElement("Title", t.Title),
                    new XElement("Description", t.Description),
                    new XElement("DueDate", t.DueDate),
                    new XElement("IsCompleted", t.IsCompleted)
                ))
            )
        );

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(xmlDoc.ToString(), System.Text.Encoding.UTF8, "application/xml")
        };
        return response;
    }

    private bool TaskExists(int id)
    {
        return db.Tasks.Count(e => e.Id == id) > 0;
    }
}