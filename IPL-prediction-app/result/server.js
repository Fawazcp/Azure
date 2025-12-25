const express = require("express");
const path = require("path");
const { Pool } = require("pg");

const app = express();

// Serve static files (for IPL background image)
app.use(express.static(path.join(__dirname, "public")));

const dbHost = process.env.DB_HOST || "db";
const dbUser = process.env.DB_USER || "postgres";
const dbPassword = process.env.DB_PASSWORD || "postgres";
const dbName = process.env.DB_NAME || "iplpredictions";

const pool = new Pool({
  host: dbHost,
  user: dbUser,
  password: dbPassword,
  database: dbName
});

app.get("/", async (req, res) => {
  try {
    const result = await pool.query(
      "SELECT team, count FROM predictions ORDER BY count DESC"
    );
    const rows = result.rows;

    let html = `
      <html>
      <head>
        <title>IPL 2026 Prediction Results</title>
        <style>
          body {
            font-family: Arial, sans-serif;
            padding: 40px;
            margin: 0;
            background: url("/ipl-bg.jpg") center center / cover no-repeat fixed;
          }
          .panel {
            background-color: rgba(255,255,255,0.9);
            padding: 30px;
            border-radius: 8px;
            display: inline-block;
          }
          table { border-collapse: collapse; margin-top: 20px; }
          th, td { border: 1px solid #ccc; padding: 8px 12px; }
          th { background: #eee; }
        </style>
      </head>
      <body>
        <div class="panel">
          <h1>IPL 2026 Winner Predictions</h1>
          <p>Refresh to see latest aggregate counts.</p>
          <table>
            <tr><th>Team</th><th>Predictions</th></tr>
    `;

    for (const row of rows) {
      html += `<tr><td>${row.team}</td><td>${row.count}</td></tr>`;
    }

    html += `
          </table>
        </div>
      </body>
      </html>
    `;

    res.send(html);
  } catch (err) {
    console.error(err);
    res.status(500).send("Error fetching results");
  }
});

app.listen(80, () => {
  console.log("Result service listening on port 80");
});

