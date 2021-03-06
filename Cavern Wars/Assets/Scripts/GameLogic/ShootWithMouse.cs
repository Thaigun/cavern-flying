﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CavernWars
{
    public class ShootWithMouse : MonoBehaviour
    {
        [SerializeField]
        private Bullet _bulletPrefab;

        [SerializeField]
        private float _timeBetweenBullets;

        [SerializeField]
        private float _bulletSpeed = 100f;

        [SerializeField]
        private Transform _bulletParent;

        private int _bulletIdStepper;

        private Camera _mainCamera;

        private float _lastBulletTime;

        // Use this for initialization
        void Start()
        {
            _lastBulletTime = Time.time;
            _mainCamera = Camera.main;
            _bulletIdStepper = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButton(0) && Time.time - _lastBulletTime > _timeBetweenBullets)
            {
                _lastBulletTime = Time.time;

                Bullet bullet = Instantiate(_bulletPrefab, _bulletParent);
                bullet.transform.position = this.transform.position;
                Vector3 cursorInWorld = _mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_mainCamera.transform.position.z));
                Vector3 aimDirection = cursorInWorld - transform.position;
                aimDirection.z = 0;
                bullet.ShootTo(aimDirection.normalized * _bulletSpeed);
                bullet.Id = ++_bulletIdStepper;

                GlobalEvents.projectileChangeDel(bullet.Id, bullet.RigidbodyComponent.position, bullet.RigidbodyComponent.velocity, (int)BulletDespawnType.NONE);

                AudioManager.Instance.PlayClip(AudioManager.Instance.shot, false);
            }
        }
    }
}