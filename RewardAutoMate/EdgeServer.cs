using OpenQA.Selenium.Edge;
using BaseTool;
using LogLevel = BaseTool.LogLevel;
using System.IO;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace RewardAutoMate
{
    public static class EdgeServer
    {
        const int MaxRewards = 40;
        const int PauseTimeMinute = 15;

        private static readonly Random Random = new();

        private static readonly string[] DefaultSearchWords =
        [
            // 科技领域
            "人工智能", "机器学习", "深度学习", "自然语言处理", "计算机视觉",
            "虚拟现实", "增强现实", "智能家居", "智能穿戴设备", "无人机",
            "新能源汽车", "自动驾驶技术", "芯片技术", "半导体产业", "量子通信",
            "区块链应用", "加密货币", "云计算服务", "大数据分析", "物联网平台",
            "5G网络建设", "6G技术研究", "操作系统创新", "编程语言趋势", "软件定义网络",
            "开源软件运动", "科技独角兽企业", "科技巨头并购", "科技伦理问题", "科技成果转化",

            // 娱乐领域
            "热门电影推荐", "奥斯卡金像奖", "戛纳电影节", "好莱坞大片", "国产电影佳作",
            "当红影视明星", "偶像团体新动态", "音乐颁奖典礼", "流行音乐榜单", "说唱音乐潮流",
            "舞蹈综艺节目", "喜剧小品热点", "动漫新番排行", "游戏电竞大赛", "网络小说爆款",
            "明星绯闻八卦", "影视翻拍争议", "音乐版权纠纷", "网红直播带货", "娱乐行业整顿",

            // 体育领域
            "足球世界杯", "欧洲杯足球赛", "篮球NBA赛事", "奥运会举办动态", "冬奥会精彩项目",
            "网球大满贯赛事", "田径世锦赛", "游泳锦标赛", "乒乓球世界冠军赛", "羽毛球公开赛",
            "高尔夫球职业赛", "赛车锦标赛", "拳击重量级对决", "体操全能冠军", "跳水明星风采",
            "体育明星转会", "体育赛事转播权", "全民健身热潮", "体育场馆建设", "体育产业发展",

            // 文化领域
            "古典诗词新解读", "文学名著翻拍", "书法艺术展览", "绘画流派探讨", "雕塑艺术新作",
            "博物馆新展亮点", "文化遗产保护", "非物质文化传承", "历史文化名城", "民俗文化节日",
            "茶文化品鉴", "酒文化溯源", "美食文化传播", "时尚文化潮流", "摄影艺术风格",
            "文化创意产业", "文化交流活动", "文化旅游融合", "文化品牌塑造", "文化市场监管",

            // 社会领域
            "教育双减政策", "高校招生改革", "职业教育发展", "就业市场形势", "创业扶持政策",
            "住房保障体系", "房地产市场调控", "医疗卫生改革", "养老服务模式", "社会保障制度",
            "交通拥堵治理", "新能源交通发展", "环境保护行动", "垃圾分类推广", "生态修复工程",
            "乡村振兴战略", "城市更新计划", "社区治理创新", "公益慈善事业", "社会公平正义",
            "网络安全问题", "信息隐私保护", "社交媒体管理", "舆情监测分析", "公共安全保障",

            // 健康领域
            "新冠疫情防控", "疫苗研发进展", "心理健康关注", "养生保健方法", "运动健身计划",
            "营养膳食搭配", "中医传统疗法", "康复医学发展", "医疗器械创新", "基因检测服务",
            "健康管理理念", "慢性病防治策略", "母婴健康呵护", "老年健康关怀", "美容护肤新趋势",

            // 经济领域
            "宏观经济形势", "货币政策调整", "财政政策动态", "国际贸易摩擦", "跨境电商发展",
            "金融科技应用", "资本市场波动", "企业数字化转型", "供应链安全保障", "消费市场趋势",
            "新兴产业投资", "传统产业升级", "中小企业扶持", "共享经济模式", "绿色经济发展",

            // 国际领域
            "中美关系走向", "俄乌冲突局势", "欧洲一体化进程", "中东地区局势", "亚太地区合作",
            "非洲发展机遇", "拉丁美洲经济", "国际贸易协定", "国际组织改革", "全球气候变化",

            // 政治领域
            "国内政治动态", "政府政策解读", "政治体制改革", "民主法治建设", "基层民主实践",
            "国际政治格局", "外交政策调整", "国际合作倡议", "地缘政治博弈", "国际冲突调解",

            // 持续扩展以达到 1000 个，此处为简化示意
            "新科技突破", "娱乐新热点", "体育新赛事", "文化新潮流", "社会新问题",
            "健康新观念", "经济新趋势", "国际新动态", "政治新举措", "教育新探索",
            "科技伦理规范", "娱乐行业自律", "体育精神弘扬", "文化传承创新", "社会治理现代化",
            "健康生活方式", "经济高质量发展", "国际合作共赢", "政治文明进步", "教育公平提升",
            "科技创新生态", "娱乐内容创新", "体育赛事商业化", "文化产业数字化", "社会民生改善",
            "健康医疗信息化", "经济结构优化", "国际秩序重塑", "政治体制活力", "教育质量提升",
            "科技人才培养", "娱乐市场规范", "体育设施升级", "文化交流互鉴", "社会和谐稳定",
            "健康养老服务", "经济开放合作", "国际和平与安全", "政治决策科学化", "教育创新驱动",
            "科技成果共享", "娱乐消费升级", "体育竞技水平", "文化品牌打造", "社会公平正义",
            "健康科普宣传", "经济风险管理", "国际发展合作", "政治参与度", "教育资源均衡",
            "科技基础设施", "娱乐行业融合", "体育文化传播", "文化遗产活化", "社会治理效能",
            "健康管理体系", "经济增长动力", "国际合作机制", "政治民主建设", "教育质量保障",
            "科技应用场景", "娱乐创新模式", "体育产业生态", "文化旅游体验", "社会民生保障",
            "健康服务质量", "经济结构调整", "国际合作平台", "政治制度优势", "教育人才培养",
            "科技安全保障", "娱乐市场监管", "体育赛事创新", "文化传承保护", "社会治理能力",
            "健康科技创新", "经济可持续发展", "国际合作领域", "政治文明建设", "教育改革深化",
            "科技企业竞争力", "娱乐文化内涵", "体育精神传承", "文化产业升级", "社会公平保障",
            "健康生活促进", "经济发展模式", "国际合作机遇", "政治参与渠道", "教育资源配置",
            "科技研发投入", "娱乐消费体验", "体育产业活力", "文化旅游融合度", "社会民生福祉",
            "健康服务创新", "经济增长潜力", "国际合作空间", "政治制度自信", "教育质量提升工程",
            "科技成果转化效率", "娱乐行业发展趋势", "体育赛事影响力", "文化遗产保护力度", "社会治理创新实践",
            "健康管理服务模式", "经济发展新动能", "国际合作新机制", "政治民主发展道路", "教育改革新举措",
            "科技企业创新生态", "娱乐文化创新活力", "体育精神弘扬途径", "文化产业数字化转型", "社会公平正义保障体系",
            "健康科技创新能力", "经济可持续发展路径", "国际合作领域拓展", "政治文明建设新高度", "教育改革深化方向",
            "科技人才队伍建设", "娱乐市场规范发展", "体育赛事商业化运作", "文化旅游融合发展战略", "社会民生改善行动计划",
            "健康服务质量提升", "经济结构优化升级", "国际合作平台搭建", "政治制度优势彰显", "教育质量保障体系完善",
            "科技应用场景拓展", "娱乐创新模式探索", "体育产业生态构建", "文化旅游体验升级", "社会治理效能提升行动",
            "健康管理体系完善", "经济增长动力转换", "国际合作机制创新", "政治民主建设新实践", "教育改革深化实践",
            "科技安全保障体系", "娱乐市场监管强化", "体育赛事创新发展", "文化传承保护工程", "社会治理能力现代化建设",
            "健康科技创新突破", "经济可持续发展战略", "国际合作领域扩大", "政治文明进步新台阶", "教育改革深化目标",
            "科技企业竞争力提升", "娱乐文化内涵挖掘", "体育精神传承创新", "文化产业升级转型", "社会公平保障机制",
            "健康生活促进计划", "经济发展模式创新", "国际合作机遇把握", "政治参与渠道拓宽", "教育资源配置优化",
            "科技研发投入加大", "娱乐消费体验升级", "体育产业活力激发", "文化旅游融合度提升", "社会民生福祉增进",
            "健康服务创新发展", "经济增长潜力释放", "国际合作空间拓展", "政治制度自信增强", "教育质量提升工程推进"
        ];

        /// <summary>
        /// 随机获取一个搜索词
        /// </summary>
        /// <returns></returns>
        private static string GetRandomSearchWord() => DefaultSearchWords[Random.Next(DefaultSearchWords.Length)];

        private static readonly string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// 生成指定长度的包含大写字母、小写字母和数字的随机字符串
        /// </summary>
        /// <param name="_length"></param>
        /// <returns></returns>
        private static string GenerateRandomString(int _length)
        {
            string _result = "";
            for (int i = 0; i < _length; i++)
            {
                _result += Characters[Random.Next(Characters.Length)];
            }
            return _result;
        }

        public static async Task<uint> AutoSearch()
        {
            //EdgeDriverService _service = EdgeDriverService.CreateDefaultService("msedgedriver.exe");

            //EdgeOptions options = new EdgeOptions();
            //string _user_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "User Data");
            //options.AddArgument($"--user-data-dir={_user_data_dir}");  // 指定 Edge 用户数据文件夹路径用于登录账户信息

            //using EdgeDriver driver = new EdgeDriver(_service, options);

            ChromeDriverService _service = ChromeDriverService.CreateDefaultService("chromedriver.exe");

            ChromeOptions options = new ChromeOptions();
            string _user_data_dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Google", "Chrome", "User Data");
            options.AddArgument($"--user-data-dir={_user_data_dir}");  // 指定 chrome 用户数据文件夹路径用于登录账户信息

            using ChromeDriver driver = new ChromeDriver(_service, options);

            uint _count = 0;
            try
            {
                BaseAction.Token.ThrowIfCancellationRequested();
                for (_count = 0; _count < MaxRewards; _count++)
                {
                    if (_count != 0 && _count % 5 == 0)
                    {
                        BaseLogManager.SendLog(LogLevel.Warning, $"暂停 {PauseTimeMinute} 分钟...", true);
                        await Task.Delay(TimeSpan.FromMinutes(PauseTimeMinute), BaseAction.Token);
                    }

                    string _search_word = GetRandomSearchWord();
                    string _random_string = GenerateRandomString(4);
                    string _rand_cvid = GenerateRandomString(32);
                    string searchUrl = _count <= MaxRewards / 2
                        ? $"https://www.bing.com/search?q={Uri.EscapeDataString(_search_word)}&form={_random_string}&cvid={_rand_cvid}"
                        : $"https://cn.bing.com/search?q={Uri.EscapeDataString(_search_word)}&form={_random_string}&cvid={_rand_cvid}";

                    BaseLogManager.SendLog(LogLevel.Debug, $"【{DateTime.Today.ToShortDateString()}】【{_count + 1}/{MaxRewards}】正在搜索：{_search_word}...", true);

                    driver.Navigate().GoToUrl(searchUrl); // 打开指定 URL
                    await Task.Delay(Random.Next(1000, 30000), BaseAction.Token);

                    // 定义 JavaScript 函数实现平滑滚动
                    string smoothScrollScript = @"
                        function smoothScrollToBottom() {
                            const scrollStep = 20;
                            const scrollDuration = 10;
                            const totalHeight = document.body.scrollHeight;
                            let currentPosition = window.scrollY;

                            function step() {
                                if (currentPosition < totalHeight) {
                                    currentPosition += scrollStep;
                                    window.scrollTo(0, currentPosition);
                                    requestAnimationFrame(step);
                                }
                            }

                            requestAnimationFrame(step);
                        }

                        smoothScrollToBottom();
                    ";
                    driver.ExecuteScript(smoothScrollScript);

                    int _random_delay = Random.Next(10000, 30000);
                    await Task.Delay(_random_delay, BaseAction.Token);
                }

                BaseLogManager.SendLog(LogLevel.Info, $"【{DateTime.Today.ToShortDateString()}】执行完成", true);
                return _count;
            }
            catch (OperationCanceledException) { return _count; }
            catch (Exception ex)
            {
                BaseLogManager.SendLog(LogLevel.Error, $"{ex.Message}", $"{ex}", true);
                return _count;
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}
