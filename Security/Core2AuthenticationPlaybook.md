# Core 2 Autentication Playbook

## 3 Local Logins

Nuget BCrypt for hashing.

`BCrypt.HashPassword(password)`

`BCrypt.Verify(password, hash)`

Pass hardcoded params into ctor of DI registered class

```c#
var users = new Dictionary<string, string> { { "Chris", "password" }};
services.AddSingleton<IUserService>(new DummyUserService(users));
```

## 9 OICD and IS4

## 10 Securing APIs with tokens

## 11 OICD Hybrid Flow to call API on behalf of the user

## 12 Combining Different Methods in the same app

## 13 Claims Transformation

## 13 AuthZ

## 15 A Custom AuthZ service
