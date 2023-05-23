using TestTask.Controllers;

namespace UnitTest;

/// <summary>
/// Class for unit tests.
/// </summary>
public class UnitTest1
{
    /// <summary>
    /// This method tests logic of the DataModelController.MakeDiff method.
    /// </summary>
    [Fact]
    public async void MakeDiffTest()
    {
        // Arrange
        string leftString = "dGVzdA==";
        string rightString = "cmVkbw==";
        
        byte[] leftData = Convert.FromBase64String(leftString);
        byte[] rightData = Convert.FromBase64String(rightString);
        
        var responseDict = new Dictionary<string, object>
        {
            {"diffResultType", "Equals"}
        };
        var expextedDiffs = new List<Dictionary<string, int>>
        {
            new() {{"offset", 0}, {"length", 1}},
            new() {{"offset", 2}, {"length", 2}}
        };
        var expectedResult = new Dictionary<string, object>
        {
            { "diffResultType", "ContentDoNotMatch" },
            { "diffs", expextedDiffs}
        };
        
        var diffs = new List<Dictionary<string, int>>();

        //Act
        responseDict = await DataModelController.MakeDiff(leftData: leftData, rightData: rightData, responseDict: responseDict,
            diffs: diffs);
        bool result = CompareDict(responseDict, expectedResult);
        
        
        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// This is a custom method that compares expected dictionary and actual dictionary from the
    /// DataModelController.MakeDiff method for testing.
    /// Suitable only for MakeDiffTest.
    /// </summary>
    /// <param name="responseDict">Actual dictionary.</param>
    /// <param name="expectedResult">Expected dictionary.</param>
    /// <returns>Boolean value that represents is dictionaries are same and equal or not.</returns>
    private bool CompareDict(Dictionary<string, object> responseDict, Dictionary<string, object> expectedResult)
    {
        if (!responseDict.ContainsKey("diffResultType") || !responseDict.ContainsKey("diffs"))
        {
            return false;
        }

        if (responseDict["diffResultType"] != expectedResult["diffResultType"])
        {
            return false;
        }
            
        var list1 = (responseDict["diffs"] as List<Dictionary<string, int>>)!;
        var list2 = (expectedResult["diffs"] as List<Dictionary<string, int>>)!;
        if (list1.Count != list2.Count)
        {
            return false;
        }
        for (int i = 0; i < list1.Count; i++)
        {
            foreach (var key in list1[i].Keys)
            {
                if (key != "offset" && key != "length")
                    return false;
                    
                if (list1[i][key] != list2[i][key])
                    return false;
            }
        }
        return true;
    }
}