using Ginseng.Models.Queries;
using Ginseng.Mvc.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Postulate.Base.Interfaces;
using System.Data.SqlClient;

namespace Testing
{
	[TestClass]
	public class QueryTests
	{
		// helpful sources:
		// https://www.jerriepelser.com/blog/using-configuration-files-in-dotnet-core-unit-tests/
		// https://weblog.west-wind.com/posts/2018/Feb/18/Accessing-Configuration-in-NET-Core-Test-Projects

		private IConfiguration _config;

		public QueryTests()
		{
			_config = new ConfigurationBuilder().AddJsonFile("config.json").Build();
		}

		private SqlConnection GetConnection()
		{
			string connectionStr = _config.GetSection("ConnectionStrings").GetValue<string>("Default");
			return new SqlConnection(connectionStr);
		}

		private void TestQuery<TQuery>() where TQuery : ITestableQuery, new()
		{
			var qry = new TQuery();
			using (var cn = GetConnection())
			{
				foreach (var testCase in qry.GetTestCases())
				{
					testCase.TestExecute(cn);
				}
			}
		}

		[TestMethod]
		public void WorkItemsQuery()
		{
			TestQuery<OpenWorkItems>();			
		}

		[TestMethod]
		public void EventLogsQuery()
		{
			TestQuery<EventLogs>();
		}

		[TestMethod]
		public void EventSubscriptionEmailNotifications()
		{
			TestQuery<InsertEventSubscriptionEmailNotifications>();
		}

		[TestMethod]
		public void EventSubscriptionTextNotifications()
		{
			TestQuery<InsertEventSubscriptionTextNotifications>();
		}

		[TestMethod]
		public void ActivitySubscriptionEmailNotifications()
		{
			TestQuery<InsertActivitySubscriptionEmailNotifications>();
		}

		[TestMethod]
		public void ActivitySubscriptionTextNotifications()
		{
			TestQuery<InsertActivitySubscriptionTextNotifications>();
		}

		[TestMethod]
		public void ProjectInfoQuery()
		{
			TestQuery<ProjectInfo>();
		}

		[TestMethod]
		public void PendingNotificationsQuery()
		{
			TestQuery<PendingNotifications>();
		}

		[TestMethod]
		public void OrgUserByNameQuery()
		{
			TestQuery<OrgUserByName>();
		}

		[TestMethod]
		public void NextPriorityQuery()
		{
			TestQuery<NextPriority>();
		}

	}
}
