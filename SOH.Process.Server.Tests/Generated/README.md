Re-/Generate the code for the OpenAPI Swagger Theraflow API

Be sure the server is running and the ``swagger.json`` is available at the following path:

https://localhost:7003/swagger/v1/swagger.json

> Note that the version number "v1" can be changed with the development progress.


Ensure you have ``nswag`` installed. We recommend to install the NPM global tooling using node.js as
described [here](https://www.npmjs.com/package/nswag).

To **install** nswag on your computer:

```bash
npm install nswag -g
```

### Generate teh API client code.

Ensure you are within the folder `Generated` and you have access to the `nswag.json` configuration file.

Run the following command within the folder `Generated`:

```bash
nswag run nswag.json
```

> In case that Windows is restricting the execution of scripts (e.g., you run the command within Rider) use an **
> external** terminal)