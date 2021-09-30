# Fundamentals

(demo uses Docker eShopOnContainers. Ex file are just pdfs)

## 2 Intro

beware of distributed monoliths = woe if shared concerns / tight coupling that cause a change in one to be a change in n.

Autonomous

Independently deployable

## 3 Architecting

Each has their own datastore. Important.
We lose joins, lose transactions across boundaries. temporary inconsistencies.

One MS might cache the data from another (ag cache User)

### MS Boundaries

Hard. Easier if starting from a monolith.

Look for seams in the database / tables not joined to others.

MS's should be designed around busines capabilities. - DDD bounded contexts, each with own ubiquitous language.

(See DD Vlagimir on PS)

Avoid anemic CRUD services where logic is elsewhere. Avoid circular dependencies.

## 4 Building

Containers are best. Alt, several MS on a VM is ok. PaaS is good.

### 4.2 Docker Demo

install Docker Desktop (www.docker.com)

clone eshop from github

Enable WSL 2 based engine in docker desktop. update win firewall to open ports (using a ps script provided by eshop).

Docker Compose build  `docker-compose build` // builds container for each MS using docker-compose.yml file.

`docker-compose up` // starts each ms. Just follow eshop instructions basically.

MS have their own repo.

CI ofc. runs unit tests. run service level tests: test a single service in isolation (mock collaborators). e2e: automate key journies.can be fragile.

Define MS templates or "exemplar" MS to be copied - help standardize logging, health checks, authentications, configuration, build scripts etc.

`docker-compose down` // stop.

## 5 Communication

Minimize call. publish messages to shared event bus. async comms.

api gateway sits in front of microservices. clients just hit gateway. gateway handles security.

front end > api gateway > microservices > event bus (eg: az service bus ||x rabbitMQ)

back end for front end (for each client) - might recieve one request from client, but make 3 to different MS, aggregate and return.

### sync

query from frontend - gateway - catalogMS - DB. sync is fast. REST is fine.

### async

eg: submit order fron frontend. 202 Accepted (implies ok but not complete). publish to event bus. Commands (send email)/Events (order placed)

www.enterpriseintegrationpatterns.com

### Reslient Communication Patterns

expect transient errors

retries with back off (Polly)

circuit breaker

fallback to cache

### service discovery

can't harcode addresses as need flixibility.

can use a service registry. Paas will often auto allocate a dns name = easy. Kebernetes has a built in DNS which does stuff like this.

## 6 securing

### Authentication (who)

... Use an identity server.

### Authorization (what)

beware Confused Deputy:

client calls Ordering MS, which call Payment MS.

Client to Ordering includes AuthO for client, but Order to Payment does not (just client credentials). solution - OnBehalfOf tokens

### Network

virtual networks and api gateways help here. Api Gatway in front of virtual network, deciding which backend MS have public client access.

Defence in Depth principle (lots of layers):

## 7 Delivery

ps: dan whalin - k8 for beginners supposed to be good.

upgrading one MS should not mean others have to be deployed.

blue-green swap (lb swaps from a to b.)

rolling upgrade (if lots of versions)

## Monitoring

eshop is good example, sends logs to Seq (prounounced seek. costs but free for dev) (centrealised service)

uses health checks.

FIN.

# Building Microservices

## 3 MS Domain Logic

+ Transaction script
+ Domain model
+ Table module

### MS code responsibilities

+ incoming requests: http requests, queue messages
+ domain logic
+ data access : commands, queries,
+ integrate: publish message, call other ms or third party services

#### domain logic patterns

from Patters of enterprise application architecture book.

Transaction Script: simple

Domain Model: OO see Ordering MS

Table Module:

#### data access patterns

orm, document dbs

cqrs, event sourcing (store state changes as events)

#### Service Layer

Backend For Frontend. Can even have customer enpoints, eg /specialOffers which act as a front for retrieving an aggregation of more generic services.

#### Transaction Script Pattern

A single procedure handles each request from the presentation layer. Simple. Might often mix data access, logic, presentation etc.

eg: Add New Item to the Basket

+ fetch basket from fb
+ basket validation
+ add item
+ save basket etc.

As logic increases, gets unweildly. Hard to test. Duplication. Good for simple apps. See eshopContainer Basket microservice > BasketController.CheckoutAsync

#### Domain Model Pattern

centralize logic. See Ordering microservice in eshopC, it's a DDD example.

Data mapping via data mapping layer (poco to domain. orm to domain etc )

See ShipOrderCommandHandler

##### CQRS

Commands should not return data???!

#### Table Module Pattern

Class responsible for all rows in a table, so Orders rather than Order.

God knows why you would use this.

### 5 AuthN AuthO

blah. credentials for access token etc..

eshopC is IS4, bah.
