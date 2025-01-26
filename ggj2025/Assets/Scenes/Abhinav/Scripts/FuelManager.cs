using UnityEngine;
using UnityEngine.UI;

public class FuelManager : MonoBehaviour
{
	public Slider fuelSlider; // UI Slider to display fuel
	public float maxFuel = 100f; // Maximum fuel capacity
	public float refuelRate = 10f; // Rate at which fuel refills per second when grounded
	public float maxLaunchFuelCost = 30f; // Fuel cost for a full launch

	private float currentFuel; // Current fuel amount

	private void Start()
	{
		currentFuel = maxFuel; // Start with full fuel
		fuelSlider.maxValue = maxFuel;
		fuelSlider.value = currentFuel;
	}

	private void Update()
	{
		// Gradually refuel when the player is grounded
		HoldAndShoot holdAndShoot = GetComponent<HoldAndShoot>();
		if (holdAndShoot.isGrounded && currentFuel < maxFuel)
		{
			currentFuel += refuelRate * Time.deltaTime;
			currentFuel = Mathf.Min(currentFuel, maxFuel); // Clamp to maxFuel
			fuelSlider.value = currentFuel; // Update slider
		}
	}

	// Public method to calculate max allowed launch speed based on remaining fuel
	public float GetMaxAllowedLaunchSpeed(float maxLaunchSpeed)
	{
		if (currentFuel <= 0)
		{
			// If out of fuel, allow a minimum launch speed to prevent clamping in only one direction
			return 0; // Return 0 to prevent launch
		}

		// Calculate the maximum allowable launch speed based on remaining fuel
		float maxFuelRatio = currentFuel / maxLaunchFuelCost;
		return maxLaunchSpeed * Mathf.Min(maxFuelRatio, 1f); // Cap at maxLaunchSpeed
	}

	// Public method to consume fuel after launching 
	public void ConsumeFuel(float launchSpeed, float maxLaunchSpeed)
	{
		// Calculate fuel cost based on the actual launch speed
		float fuelCost = (launchSpeed / maxLaunchSpeed) * maxLaunchFuelCost;
		currentFuel -= fuelCost;
		currentFuel = Mathf.Max(0, currentFuel); // Ensure fuel doesn't go below 0
		fuelSlider.value = currentFuel; // Update slider
	}

	// Public method to get the current fuel percentage
	public float GetFuelPercentage()
	{
		return (currentFuel / maxFuel) * 100f;
	}
}
