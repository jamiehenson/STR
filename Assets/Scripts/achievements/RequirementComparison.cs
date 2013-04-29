using UnityEngine;
using System.Collections;

public abstract class RequirementComparison {
	protected int val;

	protected RequirementComparison(int v) {
		val = v;
	}

	public abstract bool compare(int newVal);

	public static RequirementComparison newRequirementComparision(string op, int v){
		switch (op) {
			case("<"):
				return new RequirementComparisonLT(v);
				break;
			case("<="):
				return new RequirementComparisonLE(v);
				break;
			case(">"):
				return new RequirementComparisonGT(v);
				break;
			case(">="):
				return new RequirementComparisonGE(v);
				break;
		}
		return null;
	}
}

public class RequirementComparisonLT : RequirementComparison {
	public RequirementComparisonLT(int v) : base(v) {}
	public override bool compare(int newVal) {return (val > newVal);}
}

public class RequirementComparisonLE : RequirementComparison {
	public RequirementComparisonLE(int v) : base(v) {}
	public override bool compare(int newVal) {return (val >= newVal);}
}

public class RequirementComparisonGT : RequirementComparison {
	public RequirementComparisonGT(int v) : base(v) {}
	public override bool compare(int newVal) {return (val < newVal);}
}

public class RequirementComparisonGE : RequirementComparison {
	public RequirementComparisonGE(int v) : base(v) {}
	public override bool compare(int newVal) {return (val <= newVal);}
}