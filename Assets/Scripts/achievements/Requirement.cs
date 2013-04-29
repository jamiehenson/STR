using UnityEngine;
using System.Collections;

public abstract class Requirement {
	protected RequirementComparison comparison;

	public Requirement(RequirementComparison c) {
		comparison = c;
	}

	public abstract bool compare(Metrics m);

	public static Requirement newRequirement(string metric, string op, int v){
		RequirementComparison c = RequirementComparison.newRequirementComparision(op, v);

		switch (metric) {
			case("shots"):
				return new RequirementShots(c);
				break;
			case("kills"):
				return new RequirementKills(c);
				break;
			case("levels"):
				return new RequirementLevels(c);
				break;
		}
		return null;
	}
}

public class RequirementShots : Requirement {
	public RequirementShots(RequirementComparison c) : base(c) {}
	public override bool compare(Metrics m) {return comparison.compare(m.shots);}
}

public class RequirementKills : Requirement {
	public RequirementKills(RequirementComparison c) : base(c) {}
	public override bool compare(Metrics m) {return comparison.compare(m.kills);}
}

public class RequirementLevels : Requirement {
	public RequirementLevels(RequirementComparison c) : base(c) {}
	public override bool compare(Metrics m) {return comparison.compare(m.levels);}
}