import { useEffect, useState } from "react";
import { GamesClient, type GameDto } from "../../core/generated-client";
import { baseUrl } from "@core/baseUrl";
import Pagination from "../Pagination";
import { SquarePen, Trash2 } from "lucide-react";
import { ActionMenu } from "@components/ActionMenu";

export default function GamesList() {
    const [games, setGames] = useState<GameDto[]>([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [totalPages, setTotalPages] = useState(1);

    const pageSize = 10;

    const fetchGames = (page: number) => {
        const client = new GamesClient(baseUrl, {
            fetch: async (url, init) => {
                init = init ?? {};
                init.headers = {
                    ...(init.headers ?? {}),
                    Authorization: `Bearer ${localStorage.getItem("token")}`,
                };
                return fetch(url, init);
            },
        });

        client.getAllGames(null, null, page, pageSize)
            .then((res) => {
                setGames(res.items);
                setTotalPages(Math.ceil(res.total / pageSize));
            })
            .catch(console.error);
    };

    useEffect(() => {
        fetchGames(currentPage);
    }, [currentPage]);

    const handleDelete = (game: GameDto) => {
        return undefined;
    };

    const handleEdit = (game: GameDto) => {
        return undefined;
    };

    return (
        <div>
            <p className="text-3xl mb-2">Games</p>

            <div
                className="relative overflow-visible bg-neutral-primary-soft shadow-xs rounded-base border border-default w-full rounded-xl">
                <table className="w-full text-sm text-left rtl:text-right text-body">
                    <thead>
                    <tr className="bg-slate-300">
                        <th className="px-3 py-3 font-medium rounded-tl-xl">ID</th>
                        <th className="py-3 font-medium">Week</th>
                        <th className="py-3 font-medium">Year</th>
                        <th className="py-3 font-medium">Start Time</th>
                        <th className="py-3 font-medium">Revenue</th>
                        <th className="py-3 font-medium">Winning Numbers</th>
                        <th className="py-3 rounded-tr-xl"></th>
                    </tr>
                    </thead>

                    <tbody>
                    {games.map((game, index) => {
                        const isLast = index === games.length - 1;

                        return (
                            <tr
                                key={game.id}
                                className={`border-b border-default even:bg-slate-200 ${
                                    isLast ? "last:border-0" : ""
                                }`}
                            >
                                <td className={`max-w-[120px] px-3 py-2 text-ellipsis ${isLast ? "rounded-bl-xl" : ""}`}>
                                    {game.id}
                                </td>

                                <td className="px-3 py-2">{game.weekNumber}</td>

                                <td className="px-3 py-2">{game.year}</td>

                                <td className="px-3 py-2 whitespace-nowrap">
                                    {game.startTime ? new Date(game.startTime).toLocaleString() : "-"}
                                </td>

                                <td className="px-3 py-2">{game.revenue}</td>

                                <td className="px-3 py-2 max-w-[150px] overflow-hidden text-ellipsis whitespace-nowrap">
                                    {game.winningNumbers ?? "-"}
                                </td>

                                <td className={`w-12 px-3 py-2 ${isLast ? "rounded-br-xl" : ""}`}>
                                    <ActionMenu
                                        actions={[
                                            {
                                                label: "Edit",
                                                icon: <SquarePen />,
                                                onClick: () => handleEdit(game),
                                            },
                                            { separator: true },
                                            {
                                                label: "Delete",
                                                color: "#ff0000",
                                                icon: <Trash2 color="#ff0000" />,
                                                onClick: () => handleDelete(game),
                                            },
                                        ]}
                                    />
                                </td>
                            </tr>
                        );
                    })}
                    </tbody>
                </table>
            </div>

            <div className="mt-4 flex justify-center">
                <Pagination
                    currentPage={currentPage}
                    totalPages={totalPages}
                    onPageChange={setCurrentPage}
                />
            </div>
        </div>
    );
}
