import os
from flask import Flask, render_template, request, redirect
import redis

app = Flask(__name__)

redis_host = os.getenv("REDIS_HOST", "redis")
redis_port = int(os.getenv("REDIS_PORT", "6379"))

r = redis.Redis(host=redis_host, port=redis_port, db=0)

# Simple static list of IPL 2026 teams (adjust if needed)
TEAMS = [
    "Chennai Super Kings",
    "Mumbai Indians",
    "Royal Challengers Bangalore",
    "Kolkata Knight Riders",
    "Rajasthan Royals",
    "Sunrisers Hyderabad",
    "Delhi Capitals",
    "Lucknow Super Giants",
    "Gujarat Titans",
    "Punjab Kings"
]

@app.route("/", methods=["GET", "POST"])
def index():
    if request.method == "POST":
        team = request.form.get("team")
        if team:
            # push prediction into Redis list
            r.rpush("ipl_predictions", team)
        return redirect("/")
    return render_template("index.html", teams=TEAMS)

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=80)

