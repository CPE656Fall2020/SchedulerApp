# Scheduler Import Tools Overview
The Import Tools are a collection of components designed to allow for flexible data imports from Excel Spreadsheets into the database of device profiles used for scheduling. The Import Tools project contains components in two main categories:
1. **Command Line Import Tools**. The CLI import tools are designed for automated and scripted import processes where a database needs to be built up quickly from a collection of source data.
2. **Backend Import API Components**. The import API is designed to allow for flexible programatic data import. These components are used by the CLI tools to form the fully-standalone import tools. This API is also exported and used in the main Scheduler Application GUI for interactive data importing. 

## Import Modes
Currently, only importing data from Excel Spreadsheets using a JSON map is supported. 
### Excel Import Provider (`ExcelWorksheetImporter`)
The Excel Import Provider provides translation from data in xls and xlsx files to SQL, as controlled by a JSON map. The JSON map allows for dynamically mapping which fields in the spreadsheet correspond to which columns in the database. Map files also allow for defining constant values, which can simplify the layout of Excel sheets when many iterations of an experiment may all share common parameters. Map files also provide support for applying a scaling factor to most imported parameters, which can simplify the process when specific tools are configured to produce test-data in non-standard units (such as TI's EnergyTrace emitted energy usage in microJoules, milliVolts, and microAmps, as opposed to Joules, Volts, and Amps units used by the UM25C inline USB power meter). The format for the JSON maps is outlined in the following section.

## JSON Maps
***Sample JSON Map Format***
```json
{
  "ProfileType": "<the name of the profile type>",
  "ProfileId": "<A randomly generated GUID to identify this data entry>",
  
  "key": "<value>",
  ...
  "key": "<value>"
}
```

For the `ProfileType` field, the class-name of the profile being imported must be entered exactly as it is shown in code in the `SchedulerDatabase.Models` namespace. Currently, the available options are:
- `AESEncryptorProfile`
- `CompressorProfile`

Each imported entry must also be assigned a unique GUID to serve as a primary key in the database. If left empty, a random GUID will be assigned automatically. However, it is recommended to manually generate a random value for the GUID and store it in the Excel sheet with the data. This allows for merging corrected spreadsheets into an existing database without having to completely rebuild it, since changes can be matched and made using the unique GUID.

Since the particular columns for each profile type differ, and this program is designed to be modular for future use cases, the remainder of the JSON map format is not hard coded. As many (key, value) pairs as needed can be added to match the profile type being imported.

### Constant Values
For constant values in the JSON map, the appropriate value can be entered as it would be in any other JSON document. For example, in the `AESEncryptorProfile`, to setup a test as being marked as done with OpenSSL, the following entry can be used:
```json
{
  "ProviderName": "OpenSSL-CLI"
}
```

### Dynamic Values
To reference a particular parameter in the map file to a specific cell reference in Excel, set the value equal to a string constant with the Excel Cell Identifier. For example, to refer to Row 2, Column C, the correct identifier would be "C2", including the quotation marks. For example, to read the Average Voltage Reading for an experiment from the cell H16, the following entry would be used:
```json
{
  "AverageVoltage": "H16"
}
```

### Scaled Values
A multiplicative scaling factor can be applied to Dynamic Values in order to convert them into the expected units. Typically, all units are expected to be in SI base units, such as Volts, Amps, and Joules. For example, to convert a reading of millivolts to volts, nanoamps to amps, the following scaling entries can be used:
```json
{
  "AverageVoltage": [ "J8", 0.001 ],
  "AverageCurrent": [ "J7", 1e-9 ],
}
```

As shown, 0.01 (or 1e-3) and 1e-9 are the metric scaling factors for milli and nano, respectively. The value present in cells J7 and J8 will each be scaled by 1e-3 and 1e-9 (respectively) before being stored in the database.

## Database Setup
The Scheduler Import Tools rely upon SchedulerDatabase for all database connectivity. 