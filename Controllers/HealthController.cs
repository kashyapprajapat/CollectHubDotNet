using Microsoft.AspNetCore.Mvc;
using CollecthubDotNet.Models;
using CollecthubDotNet.Services;
using System.Diagnostics;

namespace CollecthubDotNet.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly MongoDbService _mongoDbService;

        public HealthController(MongoDbService mongoDbService)
        {
            _mongoDbService = mongoDbService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
                Version = "1.0.0",
                Uptime = GetUptime(),
                DatabaseStatus = await CheckDatabaseHealth(),
                SystemInfo = GetSystemInfo(),
                Dependencies = await CheckDependencies()
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Health check completed successfully",
                Data = healthStatus
            });
        }

        [HttpGet("ui")]
        public IActionResult GetHealthUI()
        {
            // Return the HTML content for health dashboard
            return Content(GetHealthUIContent(), "text/html");
        }

        private async Task<object> CheckDatabaseHealth()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var testCollection = _mongoDbService.GetCollection<object>("health_check");
                
                // Try to perform a simple operation
                await testCollection.EstimatedDocumentCountAsync();
                stopwatch.Stop();

                return new
                {
                    Status = "Connected",
                    ResponseTime = $"{stopwatch.ElapsedMilliseconds}ms",
                    LastChecked = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Status = "Disconnected",
                    Error = ex.Message,
                    LastChecked = DateTime.UtcNow
                };
            }
        }

        private object GetSystemInfo()
        {
            return new
            {
                MachineName = Environment.MachineName,
                ProcessorCount = Environment.ProcessorCount,
                OSVersion = Environment.OSVersion.ToString(),
                WorkingSet = GC.GetTotalMemory(false) / 1024 / 1024, // MB
                GCCollections = new
                {
                    Gen0 = GC.CollectionCount(0),
                    Gen1 = GC.CollectionCount(1),
                    Gen2 = GC.CollectionCount(2)
                }
            };
        }

        private async Task<List<object>> CheckDependencies()
        {
            var dependencies = new List<object>();

            // Check MongoDB
            var dbHealth = await CheckDatabaseHealth();
            dependencies.Add(new
            {
                Name = "MongoDB",
                Type = "Database",
                Status = ((dynamic)dbHealth).Status,
                Details = dbHealth
            });

            // Add more dependency checks as needed
            dependencies.Add(new
            {
                Name = "File System",
                Type = "Storage",
                Status = Directory.Exists("wwwroot") ? "Available" : "Unavailable",
                Details = new { Path = "wwwroot" }
            });

            return dependencies;
        }

        private string GetUptime()
        {
            var startTime = Process.GetCurrentProcess().StartTime;
            var uptime = DateTime.Now - startTime;
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        private string GetHealthUIContent()
        {
            return @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>CollectHub Health Dashboard</title>
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
            color: #333;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .header {
            text-align: center;
            margin-bottom: 40px;
            animation: fadeInDown 0.8s ease-out;
        }

        .header h1 {
            color: white;
            font-size: 3rem;
            margin-bottom: 10px;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.3);
        }

        .header p {
            color: rgba(255,255,255,0.9);
            font-size: 1.2rem;
        }

        .dashboard {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 30px;
            margin-bottom: 40px;
        }

        .card {
            background: rgba(255,255,255,0.95);
            border-radius: 20px;
            padding: 30px;
            box-shadow: 0 20px 40px rgba(0,0,0,0.1);
            backdrop-filter: blur(10px);
            border: 1px solid rgba(255,255,255,0.2);
            transition: all 0.3s ease;
            animation: fadeInUp 0.8s ease-out;
        }

        .card:hover {
            transform: translateY(-10px);
            box-shadow: 0 30px 50px rgba(0,0,0,0.15);
        }

        .card-header {
            display: flex;
            align-items: center;
            margin-bottom: 20px;
        }

        .card-icon {
            width: 50px;
            height: 50px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            margin-right: 15px;
            animation: pulse 2s infinite;
        }

        .status-healthy .card-icon {
            background: linear-gradient(135deg, #4CAF50, #45a049);
            color: white;
        }

        .status-warning .card-icon {
            background: linear-gradient(135deg, #FF9800, #f57c00);
            color: white;
        }

        .status-error .card-icon {
            background: linear-gradient(135deg, #f44336, #d32f2f);
            color: white;
        }

        .card-title {
            font-size: 1.5rem;
            font-weight: 600;
            color: #333;
        }

        .status-badge {
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 0.9rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 15px;
            display: inline-block;
        }

        .status-healthy .status-badge {
            background: linear-gradient(135deg, #4CAF50, #45a049);
            color: white;
        }

        .status-warning .status-badge {
            background: linear-gradient(135deg, #FF9800, #f57c00);
            color: white;
        }

        .status-error .status-badge {
            background: linear-gradient(135deg, #f44336, #d32f2f);
            color: white;
        }

        .metric {
            display: flex;
            justify-content: space-between;
            margin-bottom: 10px;
            padding: 8px 0;
            border-bottom: 1px solid #eee;
        }

        .metric:last-child {
            border-bottom: none;
        }

        .metric-label {
            font-weight: 500;
            color: #666;
        }

        .metric-value {
            font-weight: 600;
            color: #333;
        }

        .loading {
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 40px;
        }

        .spinner {
            width: 40px;
            height: 40px;
            border: 4px solid #f3f3f3;
            border-top: 4px solid #667eea;
            border-radius: 50%;
            animation: spin 1s linear infinite;
        }

        .refresh-btn {
            position: fixed;
            bottom: 30px;
            right: 30px;
            width: 60px;
            height: 60px;
            border-radius: 50%;
            background: linear-gradient(135deg, #667eea, #764ba2);
            border: none;
            color: white;
            font-size: 24px;
            cursor: pointer;
            box-shadow: 0 10px 20px rgba(0,0,0,0.2);
            transition: all 0.3s ease;
        }

        .refresh-btn:hover {
            transform: scale(1.1) rotate(180deg);
            box-shadow: 0 15px 30px rgba(0,0,0,0.3);
        }

        @keyframes fadeInDown {
            from { opacity: 0; transform: translateY(-30px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @keyframes fadeInUp {
            from { opacity: 0; transform: translateY(30px); }
            to { opacity: 1; transform: translateY(0); }
        }

        @keyframes pulse {
            0%, 100% { transform: scale(1); }
            50% { transform: scale(1.1); }
        }

        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }

        @media (max-width: 768px) {
            .dashboard {
                grid-template-columns: 1fr;
                gap: 20px;
            }
            
            .header h1 {
                font-size: 2rem;
            }
            
            .card {
                padding: 20px;
            }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔥 CollectHub Health Dashboard</h1>
            <p>Real-time system monitoring and health status</p>
        </div>

        <div id='dashboard' class='dashboard'>
            <div class='loading'>
                <div class='spinner'></div>
            </div>
        </div>
    </div>

    <button class='refresh-btn' onclick='loadHealthData()' title='Refresh Health Data'>
        🔄
    </button>

    <script>
        async function loadHealthData() {
            try {
                const response = await fetch('/health');
                const result = await response.json();
                const data = result.data;
                
                const dashboard = document.getElementById('dashboard');
                dashboard.innerHTML = '';

                // Overall Status Card
                const statusCard = createCard(
                    '🚀',
                    'System Status',
                    data.status,
                    [
                        { label: 'Environment', value: data.environment },
                        { label: 'Version', value: data.version },
                        { label: 'Uptime', value: data.uptime },
                        { label: 'Last Check', value: new Date(data.timestamp).toLocaleString() }
                    ],
                    'healthy'
                );
                dashboard.appendChild(statusCard);

                // Database Status Card
                const dbStatus = data.databaseStatus.status.toLowerCase() === 'connected' ? 'healthy' : 'error';
                const dbCard = createCard(
                    '🗃️',
                    'Database',
                    data.databaseStatus.status,
                    [
                        { label: 'Response Time', value: data.databaseStatus.responseTime || 'N/A' },
                        { label: 'Last Checked', value: new Date(data.databaseStatus.lastChecked).toLocaleString() }
                    ],
                    dbStatus
                );
                dashboard.appendChild(dbCard);

                // System Info Card
                const sysCard = createCard(
                    '💻',
                    'System Information',
                    'Active',
                    [
                        { label: 'Machine', value: data.systemInfo.machineName },
                        { label: 'CPUs', value: data.systemInfo.processorCount },
                        { label: 'Memory', value: data.systemInfo.workingSet + ' MB' },
                        { label: 'OS', value: data.systemInfo.osVersion }
                    ],
                    'healthy'
                );
                dashboard.appendChild(sysCard);

                // Dependencies Card
                data.dependencies.forEach(dep => {
                    const depStatus = dep.status.toLowerCase() === 'connected' || dep.status.toLowerCase() === 'available' ? 'healthy' : 'error';
                    const depCard = createCard(
                        dep.type === 'Database' ? '🗄️' : '📁',
                        dep.name,
                        dep.status,
                        Object.entries(dep.details).map(([key, value]) => ({
                            label: key,
                            value: typeof value === 'object' ? JSON.stringify(value) : value
                        })),
                        depStatus
                    );
                    dashboard.appendChild(depCard);
                });

            } catch (error) {
                const dashboard = document.getElementById('dashboard');
                dashboard.innerHTML = `
                    <div class='card status-error'>
                        <div class='card-header'>
                            <div class='card-icon'>❌</div>
                            <div class='card-title'>Error Loading Health Data</div>
                        </div>
                        <p style='color: #d32f2f; margin-top: 10px;'>${error.message}</p>
                    </div>
                `;
            }
        }

        function createCard(icon, title, status, metrics, statusClass) {
            const card = document.createElement('div');
            card.className = `card status-${statusClass}`;
            
            const metricsHtml = metrics.map(metric => `
                <div class='metric'>
                    <span class='metric-label'>${metric.label}</span>
                    <span class='metric-value'>${metric.value}</span>
                </div>
            `).join('');

            card.innerHTML = `
                <div class='card-header'>
                    <div class='card-icon'>${icon}</div>
                    <div class='card-title'>${title}</div>
                </div>
                <div class='status-badge'>${status}</div>
                <div class='metrics'>
                    ${metricsHtml}
                </div>
            `;
            
            return card;
        }

        // Auto-refresh every 30 seconds
        setInterval(loadHealthData, 30000);
        
        // Load data on page load
        loadHealthData();
    </script>
</body>
</html>";
        }
    }
}