using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.Domain;

namespace TestTask.Controllers;

/// <summary>
/// Main controller of the solution.
/// </summary>
[ApiController]
[Route("v1/diff/{id:int}")]
public class DataModelController : ControllerBase
{
    private readonly AppDbContext _context;

    public DataModelController(AppDbContext context)
    {
        _context = context;
    }
    
    /// <summary>
    /// Request that makes diff of left and right data.
    /// </summary>
    /// <param name="id">DataModel Id.</param>
    /// <returns>StatusCodeResult with diff information.</returns>
    [HttpGet]
    public async Task<IActionResult> DiffDataModelRequest(int id)
    {
        DataModel? dataModel;
        dataModel = await _context.DataModel.FindAsync(id);
        
        var responseDict = new Dictionary<string, object>
        {
            {"diffResultType", ""}
        };
        responseDict["diffResultType"] = "Equals";
        
        ActionResult response;
        
        var diffs = new List<Dictionary<string, int>>();
        
        if (dataModel == null)
        {
            response = NotFound("Data with such Id not found");
            return response;
        }

        var leftData = dataModel.Left;
        var rightData = dataModel.Right;
        if (leftData.Length != rightData.Length)
        {
            responseDict["diffResultType"] = "SizeDoNotMatch";
            response = Ok(responseDict);
            return response;
        }
        
        responseDict = await MakeDiff(leftData: leftData, rightData: rightData, responseDict: responseDict, diffs: diffs);
        
        response = Ok(responseDict);
        return response;
        
    }
    
    /// <summary>
    /// Method for making diff of data.
    /// </summary>
    /// <param name="leftData">Left byte string from DataModel.</param>
    /// <param name="rightData">Right byte string from DataModel.</param>
    /// <param name="responseDict">Dictionary for the response result body.</param>
    /// <param name="diffs">List for the found differences between data.</param>
    /// <returns>Updated dictionary for the response result body.</returns>
    public static async Task<Dictionary<string, object>> MakeDiff(byte[] leftData, byte[] rightData, Dictionary<string, object> responseDict,
        List<Dictionary<string, int>> diffs)
    {
        var start = -1;
        var length = 0;
        for (int i = 0; i < leftData.Length; i++)
        {
            if (leftData[i] != rightData[i])
            {
                if (start == -1) start = i;
                length++;

                responseDict["diffResultType"] = "ContentDoNotMatch";
            }
            else
            {
                if (start == -1) continue;
                diffs.Add(new Dictionary<string, int>
                {
                    { "offset", start },
                    { "length", length }
                });
                start = -1;
                length = 0;
            }
        }

        if (start != -1)
        {
            diffs.Add(new Dictionary<string, int>
            {
                { "offset", start },
                { "length", length }
            });
        }
        if (responseDict["diffResultType"].Equals("ContentDoNotMatch"))
        {
            responseDict.Add("diffs", diffs);
        }

        return responseDict;
    }
    
    /// <summary>
    /// Request that accepts and saves left data.
    /// If Data model not found by Id - new one will be created.
    /// </summary>
    /// <param name="id">DataModel Id.</param>
    /// <param name="request">Json with data.</param>
    /// <returns>201 StatusCode result.</returns>
    [HttpPut("left", Name = "Left")]
    public async Task<IActionResult> LeftDataModelRequest(int id, DataRequest request)
    {
        DataModel? dataModel;
        dataModel = await _context.DataModel.FindAsync(id);

        byte[] data;
        try
        { 
            data = Convert.FromBase64String(request.Data);
        }
        catch (FormatException e)
        {
            return BadRequest("Wrong data format");
        }
       

        if (dataModel != null)
        {
            dataModel.Update(left: data);
            _context.Entry(dataModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        else
        {
            dataModel = new DataModel(left: data);
            await _context.DataModel.AddAsync(dataModel);
            await _context.SaveChangesAsync();
        }
        return CreatedAtRoute("Left", new {id = dataModel.Id}, dataModel);
    }
    
    /// <summary>
    /// Request that accepts and saves right data.
    /// If Data model not found by Id - new one will be created.
    /// </summary>
    /// <param name="id">DataModel Id</param>
    /// <param name="request">Json with data.</param>
    /// <returns>201 StatusCode result.</returns>
    [HttpPut("right", Name = "Right")]
    public async Task<IActionResult> RightDataModelRequest(int id, DataRequest request)
    {
        DataModel? dataModel;
        dataModel = await _context.DataModel.FindAsync(id);

        byte[] data;
        try
        { 
            data = Convert.FromBase64String(request.Data);
        }
        catch (FormatException e)
        {
            return BadRequest("Wrong data format");
        }

        if (dataModel != null)
        {
            dataModel.Update(right: data);
            _context.Entry(dataModel).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        else
        {
            dataModel = new DataModel(right: data);
            await _context.DataModel.AddAsync(dataModel);
            await _context.SaveChangesAsync();
        }
        return CreatedAtRoute("Right", new {id = dataModel.Id}, dataModel);
    }
}