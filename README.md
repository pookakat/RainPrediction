This is a rain prediction piece of C# awesomeness. 

Historical data is an xml spreadsheet that has been converted to csv. Running the file will allow the user to
input a date by month and day or month, day and year in order to search the historical rain data for the zip 
code 27612. If no date is inputted, the program will take today's date and search the data accordingly.

Then, the magic happens, it builds a series of lists to correctly calculate the average rainfall for 
27612. As time went on, new weather stations were added to the zip code to report rainfall. The program takes this into account 
and calculates the average for the date in question if multiple weather stations reported before averaging that day in with the others.

Currently, this object is returned via the Console before the program ends its function.

It then returns the average and the date requested as  JSON object. Future plans would be to write the object to a file and 
include functionality for searching the average for a year or a month as well as asking the users if any more searches are needed.

Libraries:
System
System.Collections.Generic
System.Globalization
ChoETL
Newtonsoft.Json
Newtonsoft.Json.Linq
