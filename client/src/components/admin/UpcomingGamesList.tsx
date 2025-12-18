import {useEffect, useState} from "react";
import {
    GamesClient,
    type GameDto,
    TransactionStatus,
} from "../../core/generated-client";
import {baseUrl} from "@core/baseUrl";
import Pagination from "../Pagination";
import {
    CheckCheck,
    CircleSlash,
    Mail, SquareArrowOutUpRight,
    SquarePen,
    Trash2,
    Wallet,
} from "lucide-react";
import {ActionMenu} from "@components/ActionMenu";
import {useTranslation} from "react-i18next";
import getGameClient from "@core/clients/gameClient.ts";
import {Toaster} from "react-hot-toast";
import {useNavigate} from "react-router-dom";

export default function UpcomingGamesList() {
    const {t} = useTranslation();
    const navigate = useNavigate();
    const [games, setGames] = useState<GameDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [pageSize, setPageSize] = useState(10);
    const [totalItems, setTotalItems] = useState(0);

    const fetchGames = (page: number) => {
        const client = getGameClient();

        client
            .getAllUpcomingGames(null, null, page, pageSize)
            .then((res) => {
                setGames(res.items);
                setTotalPages(Math.ceil(res.total / pageSize));
                setTotalItems(res.total);
            })
            .catch(console.error);
    };

    useEffect(() => {
        fetchGames(currentPage);
    }, [currentPage, pageSize]);

    if (games.length > 1) {
        return (
            <div>
                <Toaster position="top-center"/>
                <div
                    className={"flex flex-row items-center justify-between w-full pb-8"}
                >
                    <p className={"text-white text-3xl mb-2 mx-0 p-0"}>
                        {t("upcoming_games")}
                    </p>
                </div>

                {/* Filters */}
                <div
                    className={
                        "flex flex-row justify-end bg-gray-800 rounded-2xl p-6 border border-gray-700 mb-6"
                    }
                >
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
                {games.length > 0 ? (
                    <div
                        className="relative overflow-visible bg-gray-800 rounded-2xl border border-gray-700 text-white">
                        <table className="w-full text-sm text-left rtl:text-right text-body">
                            <thead>
                            <tr className={"px-2 bg-gray-700"}>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tl-xl"
                                    }
                                >
                                    {t("week")}
                                </th>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("year")}
                                </th>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("game:revenue")}
                                </th>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("game:bets")}
                                </th>
                                <th
                                    scope="col"
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider"
                                    }
                                >
                                    {t("game:winning_numbers")}
                                </th>
                                <th
                                    className={
                                        "px-6 py-4 text-left text-xs font-medium text-gray-400 uppercase tracking-wider rounded-tr-xl"
                                    }
                                >
                                    {t('details')}
                                </th>
                            </tr>
                            </thead>
                            <tbody className={"divide-y divide-gray-700"}>
                            {games.map((game, index) => {
                                return (
                                    <tr key={game.id} className="">
                                        <td className="px-3 py-3">
                                            <span className="font-mono text-sm">
                                              {game.weekNumber}
                                            </span>
                                        </td>

                                        <td className="px-3 py-3">
                                            <span className="font-mono text-sm">{game.year}</span>
                                        </td>

                                        <td className="px-3 py-3">{game?.revenue}</td>

                                        <td className="px-3 py-3">{game?.bets.length}</td>

                                        <td className="px-3 py-3">
                                            {game.winningNumbers ?? t("not_drawn_yet")}
                                        </td>

                                        <td>
                                            <ActionMenu dropdown={false} actions={[
                                                {
                                                    label: t("game:game_details"),
                                                    color: "#ffffff",
                                                    icon: <SquareArrowOutUpRight color="#ffffff"/>,
                                                    onClick: () =>
                                                        navigate(
                                                            `/admin/game-details/${game.id}`,
                                                        ),
                                                }
                                            ]}/>
                                        </td>
                                    </tr>
                                );
                            })}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <div className="flex flex-col justify-center items-center h-full text-3xl text-white py-16">
                        <Wallet size={50} className="mb-4"/>
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
                            perPage={pageSize}
                            totalItems={totalItems}
                        />
                    </div>
                )}
            </div>
        );
    } else {
        return (
            <div className="flex flex-col justify-center items-center h-full text-3xl text-white">
                <CircleSlash size={50}/>
                <span>{t("no_games_found")}</span>
            </div>
        );
    }
}
