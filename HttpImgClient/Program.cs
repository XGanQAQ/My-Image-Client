using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // 设置文件路径
        string filePath = @"E:\GanX\DotNetProject\HttpImgClient\HttpImgClient\Resources\images.jpg"; 
        string url = "http://localhost:8080/upload";
        while (true)
        {
            Console.WriteLine("请输入上传文件的路径：");
            filePath = Console.ReadLine();
            Console.WriteLine("请选择你的上传路由：1.默认路由 2.自定义路由");
            string route = Console.ReadLine();
            if (route == "2")
            {
                Console.WriteLine("请输入你的上传路由：");
                url = Console.ReadLine();
            }
            
            Console.WriteLine("开始执行上传...");
            try
            {
                await PostImage(url, filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine("上传过程出现错误，错误信息：",e.Message);
                throw;
            }
        }
    }

    public static async Task PostImage(string url, string filePath)
    {
        // 读取文件为二进制数据
        byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

        // 创建 HttpClient 实例
        using (HttpClient client = new HttpClient())
        {
            // 设置请求的目标 URL
            Uri requestUri = new Uri(url);

            // 创建 HttpContent，并将文件数据作为字节流发送
            using (ByteArrayContent content = new ByteArrayContent(fileBytes))
            {
                // 设置请求头 Content-Type 为 application/octet-stream
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "file",
                    FileName = Path.GetFileName(filePath)
                };

                Console.WriteLine("文件大小");
                Console.WriteLine(content.Headers.ContentLength);

                Console.WriteLine("开始上传文件...");
                
                // 发送 POST 请求
                HttpResponseMessage response = await client.PostAsync(requestUri, content);

                
                
                // 处理响应
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("文件上传成功");
                    Console.WriteLine("服务器答复："+ await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Console.WriteLine($"上传失败，状态码: {response.StatusCode}");
                }
            }
        }
    }
    
    
    public static void ParseHttpMessage(string httpMessage)
    {
        string[] lines = httpMessage.Split("\r\n");
        string firstLine = lines[0];
        Dictionary<string, string> headers = new Dictionary<string, string>();

        // 解析请求行或状态行
        if (firstLine.StartsWith("HTTP")) // 响应报文
        {
            string[] parts = firstLine.Split(' ');
            Console.WriteLine($"Response: {parts[0]} {parts[1]} {parts[2]}");
        }
        else // 请求报文
        {
            string[] parts = firstLine.Split(' ');
            Console.WriteLine($"Request: {parts[0]} {parts[1]} {parts[2]}");
        }

        // 解析头部
        int i = 1;
        while (!string.IsNullOrWhiteSpace(lines[i]))
        {
            string[] headerParts = lines[i].Split(": ", 2);
            headers[headerParts[0]] = headerParts[1];
            i++;
        }

        // 输出头部字段
        foreach (var header in headers)
        {
            Console.WriteLine($"{header.Key}: {header.Value}");
        }

        // 解析消息体
        int emptyLineIndex = httpMessage.IndexOf("\r\n\r\n");
        if (emptyLineIndex != -1 && emptyLineIndex + 4 < httpMessage.Length)
        {
            string body = httpMessage.Substring(emptyLineIndex + 4);
            Console.WriteLine("Body:");
            Console.WriteLine(body);
        }
    } 
}
