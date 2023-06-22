# ElasticSearchAPI

Project was made using .net core webapi.
The whole project was made by me except the docker part which was made by my friend Cucu Antonia.

I made this api to use in a greater application for our college project. 
What can it do?

  1) Connects do a cloud hosted elasticsearch database
  
  2) Has the following endpoints: 
  
    - http://localhost:7193/get_data?data_type=movie&snippet=false
      where
        - snippet refers to how much data your get out of the database : 10 objects (true) / all the objects (false)
        - data_type refers to the index used for searching considering the fact that I store each type of object on a different index
      
    - http://localhost:7193/get_meta
      where 
        - the endpoint returns the database type and a key-value pair of the database name and a property list string which contains basically the mapping of the database
        
The format of the responses is JsonLd. The project also contains a data validation function for null properties but I commented it because I don't really use it at the moment.
