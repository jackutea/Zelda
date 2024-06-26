using System;
using UnityEngine;

namespace Zelda {

    public static class RoleDomain {

        public static RoleEntity Spawn(GameContext ctx, int typeID) {

            RoleEntity role = GameFactory.Role_Create(ctx.assets, ctx.idService, typeID);
            role.hp = 50;
            role.hpMax = 100;

            role.OnCollisionEnterHandle = OnCollisionEnter;

            role.OnTriggerEnterHandle = (role, other) => {
                OnTriggerEnter(ctx, role, other);
            };

            // UI
            ctx.ui.HpBar_Open(role.id, role.hp, role.hpMax);

            ctx.roleRepository.Add(role);
            return role;
        }

        public static void Attr_RestoreHp(GameContext ctx, RoleEntity role, int hpAddition) {
            role.hp += hpAddition;
            if (role.hp > role.hpMax) {
                role.hp = role.hpMax;
            }
            Debug.Log("Role: " + role.id + " Hp: " + role.hp + "/" + role.hpMax);
        }

        public static void UpdateHUD(GameContext ctx, RoleEntity role, Vector3 cameraForward) {
            ctx.ui.HpBar_UpdatePosition(role.id, role.hp, role.hpMax, role.transform.position + Vector3.up * 2.3f, cameraForward);
        }

        static void OnCollisionEnter(RoleEntity role, Collision other) {
            if (other.gameObject.CompareTag("Ground")) {
                role.SetGround(true);
            }
        }

        static void OnTriggerEnter(GameContext ctx, RoleEntity role, Collider other) {
            LootEntity loot = other.GetComponent<LootEntity>();
            if (loot != null) {

                // 1. 把物品添加到背包里
                bool isPicked = role.bagCom.Add(loot.itemTypeID, loot.itemCount, () => {
                    BagItemModel item = new BagItemModel();
                    // 从模板表里读取物品信息
                    item.id = ctx.idService.itemIDRecord++;
                    item.typeID = loot.itemTypeID;
                    item.count = loot.itemCount;
                    item.isConsumable = true;
                    item.isEatable = true;
                    item.eatRestoreHp = 30;
                    return item;
                });

                // 2. 移除 Loot
                if (isPicked) {
                    LootDomain.Unspawn(ctx, loot);
                } else {
                    // 弹窗/浮字提示: 背包满了
                    Debug.LogWarning("背包满了");
                }

                // 3. 如果背包是打开着的, 则刷新背包
                BagDomain.Update(ctx, role.bagCom);

            }
        }

    }
}