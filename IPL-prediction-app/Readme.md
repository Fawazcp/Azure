<img width="4200" height="3000" alt="image" src="https://github.com/user-attachments/assets/3f18f3c6-ba1c-4514-98c2-4685d9c618d2" />


<img width="2848" height="1600" alt="image" src="https://github.com/user-attachments/assets/783a6323-af47-4693-82b9-abc953150cfe" />


# Stack overview
User (Browser) at the top, sending HTTP requests to both the prediction UI and result UI.

Prediction service (Python/Flask) sends predictions into Redis.

Worker service (.NET) reads predictions from Redis, aggregates them, and writes counts into Postgres.

Result service (Node.js/Express) reads aggregated data from Postgres and displays it to the user.

# Data flow
User → Prediction service → Redis (stores raw team predictions).

Worker ↔ Redis (consumes queue) → Postgres (stores team and count).

Result service → Postgres (reads aggregated counts) → User (results page).

# Deployment note
The same microservice topology runs either:

As Docker Compose services on your local machine, or

As Kubernetes (Minikube) Deployments and Services (prediction, result, worker, redis, db) in the ipl-prediction namespace.
