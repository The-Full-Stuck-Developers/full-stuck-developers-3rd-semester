import { Clock, GamepadIcon, Users } from "lucide-react";
import { useEffect, useState } from "react";
import getUserClient from "@core/clients/userClient.ts";
import getTransactionsClient from "@core/clients/transactionClient.ts";
import getHealthClient from "@core/clients/healthClient.ts";
import { useTranslation } from "react-i18next";

export default function Dashboard() {
  const { t } = useTranslation();
  const [usersCount, setUsersCount] = useState<number>(0);
  const [pendingTransactionsCount, setPendingTransactionsCount] =
    useState<number>(0);
  const [healthStatus, setHealthStatus] = useState<object>({
    app: null,
    db: null,
  });

  const healthCheck = async () => {
    const client = getHealthClient();
    try {
      // Run both requests in parallel
      const [appRes, dbRes] = await Promise.all([
        client.up(),
        client.databaseUp(),
      ]);

      // Parse both responses
      const appText = await appRes.data.text();
      const dbText = await dbRes.data.text();
      const appJson = JSON.parse(appText);
      const dbJson = JSON.parse(dbText);

      // Update state once
      setHealthStatus({ app: appJson, db: dbJson });

      console.log({ app: appJson, db: dbJson });
    } catch (err) {
      console.error("Health check error:", err);
    }
  };

  // healthCheck();

  const fetchUsersCount = () => {
    const client = getUserClient();

    client
      .getPlayersCount()
      .then((res) => {
        setUsersCount(res.count);
      })
      .catch(console.error);
  };

  const fetchPendingTransactionsCount = () => {
    const client = getTransactionsClient();
    client
      .getPendingTransactionsCount()
      .then((res) => {
        setPendingTransactionsCount(res.count);
      })
      .catch(console.error);
  };
  useEffect(() => {
    fetchUsersCount();
    fetchPendingTransactionsCount();
    healthCheck();
  }, []);

  return (
    <div>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8 text-white">
        {/* App Health Widget */}
        <div
          className={`rounded-2xl p-6 border ${
            healthStatus.app?.status === "up"
              ? "bg-green-800 border-green-700"
              : "bg-red-800 border-red-700"
          }`}
        >
          <div className="flex items-start justify-between mb-4">
            <div className="text-3xl font-bold">{t("app_status")}</div>
            <div
              className={`p-1 rounded-xl ${
                healthStatus.app?.status === "up"
                  ? "bg-green-900/30"
                  : "bg-red-900/30"
              }`}
            >
              <Clock
                className={`w-6 h-6 ${healthStatus.app?.status === "up" ? "text-green-400" : "text-red-400"}`}
              />
            </div>
          </div>
          {/*<p className="text-sm text-gray-400">App Status</p>*/}
          <p className="text-xl text-white">
            {healthStatus.app?.status === "up"
              ? t("running_smoothly")
              : `${t("error_code")}: ${healthStatus.app?.code}`}
          </p>
          <p className="text-xs mt-1 text-white">
            Checked at:{" "}
            {new Date(healthStatus.app?.timestamp).toLocaleTimeString()}
          </p>
        </div>

        {/* DB Health Widget */}
        <div
          className={`rounded-2xl p-6 border ${
            healthStatus.db?.status === "up"
              ? "bg-green-800 border-green-700"
              : "bg-red-800 border-red-700"
          }`}
        >
          <div className="flex items-start justify-between mb-4">
            <div className="text-3xl font-bold">{t("database")}</div>
            <div
              className={`p-1 rounded-xl ${
                healthStatus.db?.status === "up"
                  ? "bg-green-900/30"
                  : "bg-red-900/30"
              }`}
            >
              <Clock
                className={`w-6 h-6 ${healthStatus.db?.status === "up" ? "text-green-400" : "text-red-400"}`}
              />
            </div>
          </div>
          <p className="text-xl mt-1 text-white">
            {healthStatus.db?.status === "up"
              ? t("running_smoothly")
              : `${t("error_code")}: ${healthStatus.db?.code}`}
          </p>
          <p className="text-xs mt-1 text-white">
            Checked at:{" "}
            {new Date(healthStatus.db?.timestamp).toLocaleTimeString()}
          </p>
        </div>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6 mb-8 text-white">
        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">{usersCount}</div>
            <div className="p-3 rounded-xl bg-blue-900/30">
              <Users className="w-6 h-6 text-blue-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("total_players")}</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">{pendingTransactionsCount}</div>
            <div className="p-3 rounded-xl bg-yellow-900/30">
              <Clock className="w-6 h-6 text-yellow-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">
            {t("transactions_pending_approval")}
          </p>
          {pendingTransactionsCount > 0 && (
            <p className="text-xs mt-1 text-red-400 animate-pulse">
              {t("requires_action")}
            </p>
          )}
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">40</div>
            <div className="p-3 rounded-xl bg-purple-900/30">
              <GamepadIcon className="w-6 h-6 text-purple-400" />
            </div>
          </div>
          <p className="text-sm text-gray-400">{t("total_games")}</p>
        </div>

        <div className="bg-gray-800 rounded-2xl p-6 border border-gray-700">
          <div className="flex items-start justify-between mb-4">
            <div className="text-4xl font-bold">Next Game</div>
            <div className="p-3 rounded-xl bg-purple-900/30">
              <GamepadIcon className="w-6 h-6 text-purple-400" />
            </div>
          </div>
          {/*<p className="text-sm text-gray-400">{ t('total_games') }</p>*/}
        </div>
      </div>
    </div>
  );
}
