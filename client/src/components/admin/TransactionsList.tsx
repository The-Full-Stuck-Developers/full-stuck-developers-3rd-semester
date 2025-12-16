import { useEffect, useState } from "react";
import {
  type TransactionDto,
  TransactionStatus,
} from "@core/generated-client.ts";
import { baseUrl } from "@core/baseUrl.ts";
import Pagination from "../Pagination";
import { CheckCheck, Mail, Wallet } from "lucide-react";
import toast, { Toaster } from "react-hot-toast";
import { useTranslation } from "react-i18next";
import { ActionMenu } from "@components/ActionMenu.tsx";
import { useNavigate } from "react-router-dom";
import getTransactionsClient from "@core/clients/transactionClient.ts";

export default function TransactionsList() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [transactions, setTransactions] = useState<TransactionDto[]>([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [userSearchFilter, setUserSearchFilter] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [pageSize, setPageSize] = useState(10);

  const buildFilterString = () => {
    const filters: string[] = [];

    if (userSearchFilter) {
      filters.push(
        `User.Name@=*${userSearchFilter}|User.Email@=*${userSearchFilter}`,
      );
    }

    if (statusFilter) {
      filters.push(`Status==${statusFilter}`);
    }

    return filters.length > 0 ? filters.join(",") : null;
  };

  const fetchTransactions = (page: number) => {
    const client = getTransactionsClient();
    const filterString = buildFilterString();

    client
      .getAllTransactions(filterString, null, page, pageSize)
      .then((res) => {
        setTransactions(res.items);
        setTotalPages(Math.ceil(res.total / pageSize));
      })
      .catch(console.error);
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      setCurrentPage(1);
      fetchTransactions(1);
    }, 500);

    return () => clearTimeout(timer);
  }, [userSearchFilter, statusFilter, pageSize]);

  useEffect(() => {
    fetchTransactions(currentPage);
  }, [currentPage]);

  const handleApproveTransaction = (transaction: TransactionDto) => {
    const client = getTransactionsClient();

    client
      .approveTransaction(transaction.id!)
      .then((res) => {
        console.log(res);
        fetchTransactions(currentPage);
        toast.success("Transaction approved successfully.");
        return;
      })
      .catch((error) => {
        console.log(error);
        try {
          const errorData = JSON.parse(error.response);
          toast.error(errorData.message);
          return;
        } catch {
          toast.error(error.message || "An error occurred");
          return;
        }
      });
  };

  const getStatusLabel = (status: TransactionStatus): string => {
    return TransactionStatus[status];
  };

  const getStatusBadge = (status: TransactionStatus) => {
    const baseClasses = "px-2 py-1 rounded-full text-xs font-semibold";
    switch (status) {
      case TransactionStatus.Accepted:
        return `${baseClasses} bg-green-500/20 text-green-400`;
      case TransactionStatus.Pending:
        return `${baseClasses} bg-yellow-500/20 text-yellow-400`;
      case TransactionStatus.Rejected:
      case TransactionStatus.Cancelled:
        return `${baseClasses} bg-red-500/20 text-red-400`;
      default:
        return `${baseClasses} bg-gray-500/20 text-gray-400`;
    }
  };

  return (
    <div>
      <Toaster position="top-center" />
      <div className={"flex flex-row items-center justify-between w-full pb-8"}>
        <p className={"text-white text-3xl mb-2 mx-0 p-0"}>
          {t("transactions")}
        </p>
      </div>

      {/* Filters */}
      <div
        className={
          "flex flex-row justify-between bg-gray-800 rounded-2xl p-6 border border-gray-700 mb-6"
        }
      >
        <div className={"flex flex-row gap-4"}>
          <div>
            <input
              type="text"
              placeholder={t("placeholders:search_user")}
              value={userSearchFilter}
              onChange={(e) => setUserSearchFilter(e.target.value)}
              className="w-69 pl-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent"
            />
          </div>
          <div>
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2.5 bg-gray-700 border border-gray-600 rounded-xl text-white focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent cursor-pointer"
            >
              <option value="">{t("placeholders:all_statuses")}</option>
              <option value={TransactionStatus.Pending}>
                {TransactionStatus[TransactionStatus.Pending]}
              </option>
              <option value={TransactionStatus.Accepted}>
                {TransactionStatus[TransactionStatus.Accepted]}
              </option>
              <option value={TransactionStatus.Rejected}>
                {TransactionStatus[TransactionStatus.Rejected]}
              </option>
              <option value={TransactionStatus.Cancelled}>
                {TransactionStatus[TransactionStatus.Cancelled]}
              </option>
            </select>
          </div>
        </div>
        <div>
          <select
            className={
              "px-3 py-2 bg-gray-700 border border-gray-600 rounded-xl text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-red-500 focus:border-transparent w-18 cursor-pointer"
            }
            onChange={(e) => setPageSize(Number(e.target.value))}
          >
            <option value={10}>10</option>
            <option value={25}>25</option>
            <option value={50}>50</option>
          </select>
        </div>
      </div>

      {/* Table */}
      {transactions.length > 0 ? (
        <div className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white">
          <table className="w-full text-sm text-left rtl:text-right text-body">
            <thead>
              <tr className={"px-2 bg-gray-700"}>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl"
                  }
                >
                  {t("user")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("amount")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("mobile_pay_number")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("status")}
                </th>
                <th
                  scope="col"
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                  }
                >
                  {t("date")}
                </th>
                <th
                  className={
                    "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tr-xl"
                  }
                ></th>
              </tr>
            </thead>
            <tbody className={"divide-y divide-gray-700"}>
              {transactions.map((transaction, index) => {
                const isLast = index === transactions.length - 1;
                return (
                  <tr key={transaction.id} className="">
                    <td
                      className={`px-3 py-3 ${isLast ? "rounded-bl-xl" : ""}`}
                    >
                      <div className="flex items-center gap-2">
                        <div
                          className={
                            "w-8 h-8 min-w-8 min-h-8 rounded-full bg-gradient-to-br from-red-500 to-red-700 flex items-center justify-center text-white font-bold text-xs"
                          }
                        >
                          {transaction.user?.name
                            ?.trim()
                            .split(/\s+/)
                            .map((w) => w[0].toUpperCase())
                            .join("") || "U"}
                        </div>
                        <div className="flex flex-col">
                          <span className="text-sm font-medium">
                            {transaction.user?.name || "N/A"}
                          </span>
                          <span className="text-xs text-gray-400 flex items-center gap-1">
                            <Mail size={12} />
                            {transaction.user?.email || "N/A"}
                          </span>
                        </div>
                      </div>
                    </td>
                    <td className="px-3 py-3">
                      <div className="flex items-center gap-1 font-semibold text-green-400">
                        <span>{transaction.amount.toFixed(2)}</span>
                        <span className={"text-xs"}>kr.</span>
                      </div>
                    </td>
                    <td className="px-3 py-3">
                      <span className="font-mono text-sm">
                        #{transaction.mobilePayTransactionNumber}
                      </span>
                    </td>
                    <td className="px-3 py-3">
                      <span className={getStatusBadge(transaction.status)}>
                        {getStatusLabel(transaction.status)}
                      </span>
                    </td>
                    <td
                      className={`px-3 py-3 ${isLast ? "rounded-br-xl" : ""}`}
                    >
                      {new Date(transaction.createdAt).toLocaleString("en-GB", {
                        year: "numeric",
                        month: "short",
                        day: "numeric",
                        hour: "2-digit",
                        minute: "2-digit",
                      })}
                    </td>
                    <td>
                      <ActionMenu
                        dropdown={true}
                        actions={[
                          ...(transaction.status == TransactionStatus.Pending ||
                          transaction.status == TransactionStatus.Rejected
                            ? [
                                {
                                  label: t("approve"),
                                  color: "#00a63e",
                                  icon: <CheckCheck color="#00a63e" />,
                                  onClick: () =>
                                    handleApproveTransaction(transaction),
                                  requiresConfirmation: true,
                                },
                              ]
                            : []),
                          // ...(!transaction.user ? [
                          {
                            label: t("user") + " " + t("transactions"),
                            color: "#d0d0d0",
                            icon: <Wallet color="#d0d0d0" />,
                            onClick: () =>
                              navigate(
                                `/admin/transactions/user/${transaction.user.id}`,
                              ),
                          },
                          // ] : []),
                        ]}
                      />
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </div>
      ) : (
        <div className="flex flex-col justify-center items-center h-full text-3xl text-white py-16">
          <Wallet size={50} className="mb-4" />
          <span>{t("no_transactions_found")}</span>
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="mt-6 flex justify-center">
          <Pagination
            currentPage={currentPage}
            totalPages={totalPages}
            onPageChange={setCurrentPage}
          />
        </div>
      )}
    </div>
  );
}
