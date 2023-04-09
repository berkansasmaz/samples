using Microsoft.AspNetCore.Http.HttpResults;
using MiniExcelLibs;

namespace ExportExcel.Todos
{
	internal static class TodoApi
	{
		public static RouteGroupBuilder MapTodos(this IEndpointRouteBuilder routes)
		{
			var group = routes.MapGroup("/todos");

			group.WithTags("Todos");

			group.MapGet("/", async () => await Task.FromResult(Todo.GetList()));

			group.MapGet("/export", async Task<Results<FileStreamHttpResult, NotFound>> (bool isComplete, CancellationToken token) =>
			{
				var filteredList = Todo.GetList().Where(x => x.IsComplete == isComplete).ToList();
				if (filteredList.Count == 0)
				{
					return TypedResults.NotFound();
				}

				var memoryStream = new MemoryStream();
				await memoryStream.SaveAsAsync(filteredList, cancellationToken: token);
				memoryStream.Seek(0, SeekOrigin.Begin);
				return TypedResults.File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "todos.xlsx");
			});

			return group;
		}
	}
}
