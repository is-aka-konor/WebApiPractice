# WebApiPractice

## Hot to build & run

* To build and run this solution locally, you need to have [.Net 5 SDK installed](https://dotnet.microsoft.com/download/dotnet/5.0)
* Use your favourite console/terminal to clone the code from GitHub repo `git clone https://github.com/is-aka-konor/WebApiPractice.git`
* Change directory to the WebApiPractice folder `cd WebApiPractice`
* Start the Kestrel web server with release build `dotnet run -c Release --project WebApiPractice.Api`
* To stop the server press `ctrl+C`

## How to test
To test this API you can use your favourite API client
### Customer endpoint
1. To create a customer, you need execute  `POST https://localhost:5001/api/customers/` with a body of type `application/json`, example
```
{
  "status": "Current",
  "firstName": "Name",
  "lastName": "Family",
  "contactDetails": [
    {
      "contactDetailsType": "phone",
      "details": "+64222333311"
    },
    {
        "contactDetailsType": "email",
        "details": "some@email.com"
    }
  ]
}
```
The following customer statuses are supported `Prospective`, `Current`, `NonActive`.
The following customer contact details types are supported `Email`, `Phone`, `Website`.

2. To get a list of all customers `GET https://localhost:5001/api/customers/`
The following query parameters are supported:
- `limit` - how many records to return, it defaults to 50.
- `status` - return customers in the provided status. Supported values `Prospective`, `Current`, `NonActive`.
- `firstName` - search customers who have (contains) in their first name a provided search term.
- `lastName` - search customers who have (contains) in their last name a provided search term.
- `order` - to reverse the order, provide `desc` value. By default customers are returning from oldest created to newest. 
- `nextCursor` - pagination cursor, the value for the next page will be provided in the response if there is any.
Example: `https://localhost:5001/api/customers/?limit=2&nextCursor=OA==&status=Prospective&order=desc`

3. To get a customer by id `GET https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/`
As a part of a response, `ETag` header will be provided. You can use this tag to check resource modification or optimistic locking for updates.
If ETag value is provided in the `If-Match` header, API might return HTTP 304 Not Modified if the resource hasn't changed.

4. To update a customer `PUT https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/`
If ETag value is provided in the `If-Match` header, API might return HTTP 412 Precondition failed if the resource has changed. The body example
```
{
  "status": "Prospective",
  "firstName": "Brand New name",
  "lastName": "Brand new family"
}
```

### Notes endpoint
1. To create a note for a customer `POST https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/notes/` with a body of type `application/json`, for example
```
{
  "noteText": "Note example"
}
```
2. Get all notes for a customer `GET https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/notes/`
The following query parameters are supported:
- `limit` - how many records to return, it defaults to 50.
- `nextCursor` - pagination cursor, the value for the next page will be provided in the response if there is any.
3. Get a note for a customer by id `GET https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/notes/2d1f939e-2395-4a31-a4ad-1511fa41d1e2/`
As a part of a response, `ETag` header will be provided. You can use this tag to check resource modification or optimistic locking for updates.
If ETag value is provided in the `If-Match` header, API might return HTTP 304 Not Modified if the resource hasn't changed.
4. Update a note for a customer by id `PUT https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/notes/2d1f939e-2395-4a31-a4ad-1511fa41d1e2/` with a body of type `application/json`, for example
```
{
  "noteText": "Update note example"
}
```
If ETag value is provided in the `If-Match` header, API might return HTTP 412 Precondition failed if the resource has changed.

5. Delete a note for a customer `DELETE https://localhost:5001/api/customers/9f43c2af-3ae3-4b20-a446-50340cdffa74/notes/2d1f939e-2395-4a31-a4ad-1511fa41d1e2/`

## Further steps
The current implementation took about twenty hours of work. There are quite a few things that needs to be added
* Add authentication to the API
* Improve security with CORS Policies
* Make generic validator for optimistic locking
* Cover Note functionality with unit tests
* Add integration test
