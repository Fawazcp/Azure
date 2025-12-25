<img width="4200" height="3000" alt="image" src="https://github.com/user-attachments/assets/3f18f3c6-ba1c-4514-98c2-4685d9c618d2" />

# Stack overview
User (Browser) at the top, sending HTTP requests to both the prediction UI and result UI.

Prediction service (Python/Flask) sends predictions into Redis.

Worker service (.NET) reads predictions from Redis, aggregates them, and writes counts into Postgres.

Result service (Node.js/Express) reads aggregated data from Postgres and displays it to the user.
