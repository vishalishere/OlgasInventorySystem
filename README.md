# OlgasInventorySystem

Inventory System API

Run the solution locally by opening it up in Visual Studio 2013 and pressing the run button.
Open a tool such as Postman and try out calling the API. Here are example calls:

GET http://localhost:57016/api/products
Will return you back preexisting products you have

POST http://localhost:57016/api/products
[{
    "Label": "Chips",
    "Type": "Snack",
    "Expiration": "11/5/2017",
    "Notification": "Product is fresh"
  },
  {
    "Label": "Apples",
    "Type": "Snack",
    "Expiration": "11/5/2017",
    "Notification": "Product is fresh"
  }]

Here you can add multiple items, but make sure you are providing a List, even if you are supplying one item. 

GET http://localhost:57016/api/product
Will return you back preexisting products you have

GET http://localhost:57016/api/product/milk
Will return you back information about the milk product that you have

PUT http://localhost:57016/api/product/milk
{
    "Expiration": "11/5/2013"
}

(Make sure you are providing Json and you have Postman switched to Json)

POST http://localhost:57016/api/product
[{
    "Label": "Chips",
    "Type": "Snack",
    "Expiration": "11/5/2017",
    "Notification": "Product is fresh"
  },
  …
]

Here you can add multiple items, but make sure you are providing a List, even if you are supplying one item.

DELETE http://localhost:57016/api/product/milk

Will delete you milk product

I kept good practices in mind while creating the solution, such as TDD and object oriented design. 
The current solution uses an in memory cache to keep track of state, however, the solution can be easily extended to use another data 
structure or database, since I used dependency injection. This also allowed me to use Fakes in the tests and remove dependency on the 
data layer. 

