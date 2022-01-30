using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Database;
using Microsoft.EntityFrameworkCore;

namespace NodeNetworking.Data {
	public class NodeContext : CoreDbContext {
		public DbSet<Node> Node => Set<Node>();
	}
}
