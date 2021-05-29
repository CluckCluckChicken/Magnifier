---
title: Docs
headertitle: docs
description: Magnifier documentation home
---
You can use the Magnifier API to get/put reactions, pins, comments, etc.

# Authentication

## Get an authentication code for use

GET `/api/Auth/code`

### Example response

GET `/api/Auth/code`

#### Request headers:

```
no extra headers needed.
```

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

GET `/api/Auth/token?code={code}`

### Example response

GET `/api/Auth/token?code=ltdtroBzjjcdeWhHDCEOKYGbBWRlPQrjhhSE`

#### Request headers:

```
no extra headers needed.
```

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

## Get a user using an auth token

GET `/api/Auth/user`

### Example response

GET `/api/Auth/user`

#### Request headers:

```
Authorization: Bearer {auth token}
```

#### Code:

`200`

#### Response body:

```
{"id":"60a90f5bfd706633fd4d6fdd","username":"potatophant","scratchUser":{"_id":null,"username":"potatophant","image":"https://cdn2.scratch.mit.edu/get_image/user/16005114_60x60.png"},"isAdmin":true,"created":"2021-05-22T14:04:11.697Z","lastLogin":"0001-01-01T00:00:00Z","ipAddresses":null}
```

#### Response headers:

```
 content-type: text/plain; charset=utf-8 
 date: Wed26 May 2021 21:35:51 GMT 
 server: Kestrel 
```

# Comments

## Get an already loaded comment using its commentId

GET `/api/Comments/{commentId}`

### Example response

GET `/api/Comments/208427471`

#### Request headers:

```
no extra headers needed.
```

#### Code:

`200`

#### Response body:

```
{
  "_id": "60aec170fe30a66d65f62c1f",
  "commentId": 208427471,
  "comment": {
    "_id": null,
    "id": 208427471,
    "content": "I purchase people then swallow them whole.",
    "datetime_created": "2021-05-20T16:44:27Z",
    "author": {
      "_id": null,
      "username": "Raihan142857",
      "image": "https://cdn2.scratch.mit.edu/get_image/user/39403909_60x60.png"
    }
  },
  "reactions": [],
  "isPinned": false,
  "isReply": false,
  "replies": [
    208433554
  ]
}
```

#### Response headers:

```
 content-type: application/json; charset=utf-8 
 date: Wed26 May 2021 21:45:48 GMT 
 server: Kestrel 
```
