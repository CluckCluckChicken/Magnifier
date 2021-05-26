---
title: Docs
headertitle: docs
description: Magnifier documentation home
---
You can use the Magnifier API to get/put reactions, pins, comments, etc.

# Authentication

## Get an authentication code for use

GET `https://localhost:5001/api/Auth/code`

### Example response

GET `https://localhost:5001/api/Auth/code`

#### Code:

`200`

#### Response body:

```
ltdtroBzjjcdeWhHDCEOKYGbBWRlPQrjhhSE
```

#### Response headers:

```
 content-type: text/plain; charset=utf-8 
 date: Wed26 May 2021 21:23:55 GMT 
 server: Kestrel 
```

## Get an authentication token using that code

GET `https://localhost:5001/api/Auth/token?code={token}`

### Example response

GET `https://localhost:5001/api/Auth/token?code=ltdtroBzjjcdeWhHDCEOKYGbBWRlPQrjhhSE`

#### Code:

`200`

#### Response body:

```
im not telling you my auth token mate, but it'll be a base64 encoded JWT token, i.e a random-seeming string of characters.
```

#### Response headers:

```
 content-type: text/plain; charset=utf-8 
 date: Wed26 May 2021 21:30:07 GMT 
 server: Kestrel 
```
