using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Header("Scripts")]
    public UserDataController userDataController;
    public StartNewOrder startNewOrder;

    [Header("Data")]
    public int userGold = 0;
    public int meat = 0;
    public int iron = 0;
    public int herbs = 0;

    public IDbConnection dbConnection;

    void Start()
    {
        dbConnection = CreateAndOpenDatabase();
        IDbCommand dbCommandReadValues = dbConnection.CreateCommand();

        dbCommandReadValues.CommandText = "SELECT Gold FROM Economy";
        IDataReader dataReader = dbCommandReadValues.ExecuteReader();
        while (dataReader.Read())
        {
            userGold = dataReader.GetInt32(0);
        }
        dataReader.Close();

        dbCommandReadValues.CommandText = "SELECT Quantity FROM Resources";
        dataReader = dbCommandReadValues.ExecuteReader();
        List<int> resourcesData = new List<int>();
        while (dataReader.Read())
        {
            resourcesData.Add(dataReader.GetInt32(0));
        }
        if (resourcesData.Count == 3)
        {
            meat = resourcesData[0];
            iron = resourcesData[1];
            herbs = resourcesData[2];
        }
        dataReader.Close();

        Dictionary<string, string> adventurersDictionary = GetAdventurersDictionary(dbConnection);
        Dictionary<string, int> resourcesDictionary = GetResourcesDictionary(dbConnection);
        startNewOrder.GetPersonLists(adventurersDictionary, resourcesDictionary);

        userDataController.UpdateAllText();
    }

    private IDbConnection CreateAndOpenDatabase()
    {
        string dbUri = "URI=file:" + Application.dataPath + "/StreamingAssets/Taverna.db";
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        CreateTables(dbConnection);
        return dbConnection;
    }

    public void CreateTables(IDbConnection dbConnection)
    {
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();

        string createAllTables = @"
            CREATE TABLE IF NOT EXISTS Adventurers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Type TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Orders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AdventurerId INTEGER NOT NULL,
                OrderType TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL,
                FOREIGN KEY (AdventurerId) REFERENCES Adventurers(Id)
            );

            CREATE TABLE IF NOT EXISTS Resources (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                ResourceName TEXT NOT NULL,
                Quantity INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Economy (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Gold INTEGER NOT NULL
            );";

        dbCommandCreateTable.CommandText = createAllTables;
        dbCommandCreateTable.ExecuteReader();
    }

    private int GetAdventurerIdByName(IDbConnection dbConnection, string adventurerName)
    {
        IDbCommand dbCommandGetId = dbConnection.CreateCommand();
        dbCommandGetId.CommandText = "SELECT Id FROM Adventurers WHERE Name = @name";

        IDbDataParameter parameterName = dbCommandGetId.CreateParameter();
        parameterName.ParameterName = "@name";
        parameterName.Value = adventurerName;

        dbCommandGetId.Parameters.Add(parameterName);

        int adventurerId = -1;
        IDataReader dataReader = dbCommandGetId.ExecuteReader();
        if (dataReader.Read())
        {
            adventurerId = dataReader.GetInt32(0);
        }
        dataReader.Close();

        return adventurerId;
    }

    public void SetNewOrders(IDbConnection dbConnection, string adventurerName, string resourceName, int quantity)
    {
        int adventurerId = GetAdventurerIdByName(dbConnection, adventurerName);
        if (adventurerId != -1)
        {
            IDbCommand dbCommandSetNewOrders = dbConnection.CreateCommand();
            dbCommandSetNewOrders.CommandText = @"
            INSERT INTO Orders (AdventurerId, OrderType, IsCompleted) 
            VALUES (@adventurerId, @orderType, 0)";

            IDbDataParameter parameterAdventurerId = dbCommandSetNewOrders.CreateParameter();
            parameterAdventurerId.ParameterName = "@adventurerId";
            parameterAdventurerId.Value = adventurerId;

            IDbDataParameter parameterOrderType = dbCommandSetNewOrders.CreateParameter();
            parameterOrderType.ParameterName = "@orderType";
            parameterOrderType.Value = $"{quantity}x {resourceName}";

            dbCommandSetNewOrders.Parameters.Add(parameterAdventurerId);
            dbCommandSetNewOrders.Parameters.Add(parameterOrderType);

            dbCommandSetNewOrders.ExecuteNonQuery();
        }
    }

    public void UpdateUserData(int gold, int meat, int iron, int herbs, bool isOrderCompleted, int orderId)
    {
        using (IDbConnection dbConnection = CreateAndOpenDatabase())
        {
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = @"
            UPDATE Economy SET Gold = @gold;
            UPDATE Resources SET Quantity = @meat WHERE ResourceName = 'Meat';
            UPDATE Resources SET Quantity = @iron WHERE ResourceName = 'Iron';
            UPDATE Resources SET Quantity = @herbs WHERE ResourceName = 'Herbs';
            UPDATE Orders SET IsCompleted = @isCompleted WHERE Id = @orderId;
        ";

            IDbDataParameter goldParam = command.CreateParameter();
            goldParam.ParameterName = "@gold";
            goldParam.Value = gold;

            IDbDataParameter meatParam = command.CreateParameter();
            meatParam.ParameterName = "@meat";
            meatParam.Value = meat;

            IDbDataParameter ironParam = command.CreateParameter();
            ironParam.ParameterName = "@iron";
            ironParam.Value = iron;

            IDbDataParameter herbsParam = command.CreateParameter();
            herbsParam.ParameterName = "@herbs";
            herbsParam.Value = herbs;

            IDbDataParameter isCompletedParam = command.CreateParameter();
            isCompletedParam.ParameterName = "@isCompleted";
            isCompletedParam.Value = isOrderCompleted ? 1 : 0;

            IDbDataParameter orderIdParam = command.CreateParameter();
            orderIdParam.ParameterName = "@orderId";
            orderIdParam.Value = orderId;

            command.Parameters.Add(goldParam);
            command.Parameters.Add(meatParam);
            command.Parameters.Add(ironParam);
            command.Parameters.Add(herbsParam);
            command.Parameters.Add(isCompletedParam);
            command.Parameters.Add(orderIdParam);

            command.ExecuteNonQuery();
        }
    }



    private Dictionary<string, string> GetAdventurersDictionary(IDbConnection dbConnection)
    {
        Dictionary<string, string> adventurersDictionary = new Dictionary<string, string>();
        IDbCommand dbCommandReadAdventurers = dbConnection.CreateCommand();
        dbCommandReadAdventurers.CommandText = "SELECT Name, Type FROM Adventurers";

        IDataReader dataReader = dbCommandReadAdventurers.ExecuteReader();
        while (dataReader.Read())
        {
            string name = dataReader.GetString(0);
            string type = dataReader.GetString(1);
            adventurersDictionary[name] = type;
        }
        dataReader.Close();

        return adventurersDictionary;
    }

    private Dictionary<string, int> GetResourcesDictionary(IDbConnection dbConnection)
    {
        Dictionary<string, int> resourcesDictionary = new Dictionary<string, int>();
        IDbCommand dbCommandReadAdventurers = dbConnection.CreateCommand();
        dbCommandReadAdventurers.CommandText = "SELECT ResourceName, Quantity FROM Resources";

        IDataReader dataReader = dbCommandReadAdventurers.ExecuteReader();
        while (dataReader.Read())
        {
            string name = dataReader.GetString(0);
            int count = dataReader.GetInt32(1);
            resourcesDictionary[name] = count;
        }
        dataReader.Close();

        return resourcesDictionary;
    }
}