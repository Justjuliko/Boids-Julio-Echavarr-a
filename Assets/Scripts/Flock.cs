using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Flock {
    List<Boid> boids; // lista de boids en este caso las vacas

    public Flock() {
        boids = new List<Boid>(); 
    }

    public void Update() {
        foreach (var boid in boids) {
            boid.Update(boids);  
        }
    }

    public void AddBoid(Boid b) {
        boids.Add(b);
    }

}
